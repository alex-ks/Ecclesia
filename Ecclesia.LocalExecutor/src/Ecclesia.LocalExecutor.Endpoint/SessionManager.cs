using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecclesia.LocalExecutor.Endpoint.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Ecclesia.LocalExecutor.Endpoint
{
    public class SessionManager
    {
        private readonly Executor _executor;
        private Dictionary<Guid, ComputationGraph> _sessionGraphs;
        private Dictionary<Guid, SessionStatus> _sessions;
        private object _lockGuard = new object();
        private readonly ILogger _logger;

        private Action<string[]> GetCallBack (Guid idSession, int idOperation)
        {
            return outputs => Notify(idSession, idOperation, outputs);
        }

        public SessionManager(ILogger<SessionManager> logger, Executor executor)
        {
            _executor = executor;
            _sessionGraphs = new Dictionary<Guid, ComputationGraph>();
            _sessions = new Dictionary<Guid, SessionStatus>();
            _logger = logger;
            _logger.LogInformation("Session manager a created!");
        }

        public Guid StartSession(ComputationGraph graph)
        {
            Guid sessionId = Guid.NewGuid();
            lock (_lockGuard)
            {
                _sessionGraphs.Add(sessionId, graph);
            
                _logger.LogInformation($"Create session with id: {sessionId}");
                
                var session = new SessionStatus 
                { 
                    OperationStatus = graph.Operations
                        .Select(op => new OperationStatus { Id = op.Id, State = OperationState.Awaits })
                        .ToList(),
                    MnemonicsTable = graph.MnemonicsTable 
                };

                _sessions.Add(sessionId, session);

                LogSessionStatus(sessionId);

                List<int> availableOperations = 
                    session.GetAvailableOperationsIds(_sessionGraphs[sessionId].Dependecies);
                EnqueueOperationsExecution(sessionId, availableOperations);

                return sessionId;
            }
        }

        private void LogSessionStatus(Guid id)
        {
            IEnumerable<string> getLogParts()
            {
                yield return $"Session {id}:";
                foreach (var status in _sessions[id].OperationStatus)
                    yield return status.State.ToString();
            }

            _logger.LogInformation(string.Join(Environment.NewLine, getLogParts()));
        }

        private void LogSessionMnemonics(Guid id)
        {
            IEnumerable<string> getLogParts()
            {
                yield return $"Session {id}:";
                foreach (var pair in _sessions[id].MnemonicsTable)
                    yield return $"name: {pair.Key}, value: {pair.Value.Value}";
            }

            _logger.LogInformation(string.Join(Environment.NewLine, getLogParts()));
        }

        public void Notify(Guid sessionId, int operationId, string[] outputs)
        {
            lock (_lockGuard)
            {
                var session = _sessions[sessionId];
                var operation = session.OperationStatus[operationId];

                if (outputs != null)
                {
                    operation.SetCompleted();

                    var graph = _sessionGraphs[sessionId];
                    session.RegisterOperationResult(graph.Operations[operationId].Output, outputs);

                    LogSessionStatus(sessionId);

                    if (!session.IsCompleted())
                    {
                        var availableOperationsIds = 
                            session.GetAvailableOperationsIds(_sessionGraphs[sessionId].Dependecies);
                        EnqueueOperationsExecution(sessionId, availableOperationsIds);
                    }
                    else
                    {
                        StopSession(sessionId);
                    }
                }
                else
                {
                    operation.SetFaild();
                    StopSession(sessionId);
                }
            }
        }

        private void EnqueueOperationsExecution(Guid sessionId, List<int> availableOperations)
        {
            var graph = _sessionGraphs[sessionId];
            var session = _sessions[sessionId];

            List<Operation> operations = GetOperationsByIds(graph.Operations, availableOperations);
            
            foreach (var operation in operations)
            {
                var inputsValues = session.GetMnemonicsValues(operation.Input);
                
                session.OperationStatus[operation.Id].SetRunning();

                Action<string[]> callback = GetCallBack(sessionId, operation.Id);
                _executor.Add(operation.Name, inputsValues.ToArray(), callback);
            }
        }

        private List<Operation> GetOperationsByIds(List<Operation> allOperations, List<int> availableIds)
        {
            var availableOperation = availableIds.Select(id => allOperations.Find(x => x.Id == id));
            return availableOperation.ToList();
        }

        public void StopSession(Guid id)
        {
            lock (_lockGuard)
            {
                if (!_sessionGraphs.ContainsKey(id))
                {
                    throw new ArgumentException($"Session {id} not found!");
                }

                var session = _sessions[id];

                if (session.IsCompleted())
                {
                    _logger.LogInformation("Session {0} requested to stop after completion", id);
                }
                else if (session.IsFailed())
                {
                    foreach(var operation in session.OperationStatus.Where(op => op.State == OperationState.Awaits))
                        operation.SetAborted();
                    _logger.LogError("Session {0} requested to stop but was aborted");
                }
                else
                {
                    foreach (var operation in session.OperationStatus.Where(op => op.State == OperationState.Awaits))
                        operation.SetAborted();
                    _logger.LogInformation("Aborting session {0} by user request", id);
                }
            }
        }

        public SessionStatus GetSessionStatus(Guid idSession)
        {
            if (_sessions.ContainsKey(idSession))
                return _sessions[idSession];
            else
                throw new ArgumentException($"Session {idSession} not found!");
        }
    }
}
