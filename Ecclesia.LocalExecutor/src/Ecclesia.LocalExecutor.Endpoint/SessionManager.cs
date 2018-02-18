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
        private Dictionary<Guid, ComputationGraph> _sessionDictionary;
        private Dictionary<Guid, SessionStatus> _sessionStatus;
        private object _lockGuard = new object();
        private readonly ILogger _logger;

        private Action<string[]> GetCallBack (Guid idSession, int idOperation)
        {
            return outputs => Notify(idSession, idOperation, outputs);
        }

        public SessionManager(ILogger<SessionManager> logger, Executor executor)
        {
            _executor = executor;
            _sessionDictionary = new Dictionary<Guid, ComputationGraph>();
            _sessionStatus = new Dictionary<Guid, SessionStatus>();
            _logger = logger;
            _logger.LogInformation("Session manager a created!");
        }

        public Guid StartSession(ComputationGraph session)
        {
            Guid idSession = Guid.NewGuid();
            lock (_lockGuard)
            {
                _sessionDictionary.Add(idSession, session);
            
                _logger.LogInformation($"Create session with id: {idSession}");
                
                _sessionStatus.Add(idSession, new SessionStatus 
                { 
                    OperationStatus = session.Operations
                        .Select(op => new OperationStatus { Id = op.Id, Status = OperationState.Awaits })
                        .ToList(),
                    MnemonicsTable = session.MnemonicsTable 
                });

                LogSessionStatus(idSession);

                List<int> availableOperations = SessionUtilities.GetIDAvailableOperation(
                    _sessionStatus[idSession].OperationStatus,
                    _sessionDictionary[idSession].Dependecies);
                ExecuteOperations(idSession, availableOperations);

                return idSession;
            }
        }

        private void LogSessionStatus(Guid id)
        {
            IEnumerable<string> getLogParts()
            {
                yield return $"Session {id}:";
                foreach (var status in _sessionStatus[id].OperationStatus)
                    yield return status.Status.ToString();
            }

            _logger.LogInformation(string.Join(Environment.NewLine, getLogParts()));
        }

        private void LogSessionMnemonics(Guid id)
        {
            IEnumerable<string> getLogParts()
            {
                yield return $"Session {id}:";
                foreach (var pair in _sessionStatus[id].MnemonicsTable)
                    yield return $"name: {pair.Key}, value: {pair.Value.Value}";
            }

            _logger.LogInformation(string.Join(Environment.NewLine, getLogParts()));
        }

        public void Notify(Guid idSession, int operationId, string[] outputs)
        {
            lock (_lockGuard)
            {
                OperationStatus operationSt = _sessionStatus[idSession].OperationStatus[operationId];
                if (outputs != null)
                {
                    SessionUtilities.OperationCompleted(operationSt, outputs);

                    var session = _sessionDictionary[idSession];
                    SessionUtilities.UpdateMnemonicValues(
                        _sessionStatus[idSession].MnemonicsTable,
                        session.Operations[operationId].Output,
                        outputs);
                    LogSessionStatus(idSession);

                    if (!SessionUtilities.SessionCompleted(_sessionStatus[idSession].OperationStatus))
                    {
                        List<int> idAvailableOperation = SessionUtilities.GetIDAvailableOperation(
                            _sessionStatus[idSession].OperationStatus,
                            _sessionDictionary[idSession].Dependecies);
                        ExecuteOperations(idSession, idAvailableOperation);
                    }
                    else
                    {
                        StopSession(idSession);
                    }
                }
                else
                {
                    SessionUtilities.OperationFaild(operationSt);
                    StopSession(idSession);
                }
            }
        }

        private void ExecuteOperations(Guid idSession, List<int> idAvailableOperation)
        {
            var session = _sessionDictionary[idSession];
            var operationSession = session.Operations;
            var mnemonicsTableSession = session.MnemonicsTable;
            Console.WriteLine($"ID available operation for session - {idSession} " +
                                                        $"- is: {string.Join(',', idAvailableOperation.ToArray())}");
            List<Operation> listAvailable = GetAvailable(operationSession, idAvailableOperation);
            
            foreach (Operation operation in listAvailable)
            {
                
                List<string> inputsValues = GetInputsValues(operation.Input,mnemonicsTableSession);
                SessionUtilities.OperationRunning(_sessionStatus[idSession].OperationStatus[operation.Id]);
                Action<string[]> callback = GetCallBack(idSession, operation.Id);
                _executor.Add(operation.Name, inputsValues.ToArray(), callback);
            }
        }

        public static List<string> GetInputsValues(string[] operationInputs, Dictionary<string, MnemonicsValue> mnemonicsTable)
        {
            List<string> inputsValues = new List<string>();
            foreach (string variable in operationInputs)
            {
                inputsValues.Add(mnemonicsTable[variable].Value);
            }
            return inputsValues;
        }

        public List<Operation> GetAvailable(List<Operation> allOperation, List<int> IdAvailable)
        {
            var availableOperation = IdAvailable.Select(id => allOperation.Find(x => x.Id == id));
            
            return availableOperation.ToList();
        }

        public void StopSession(Guid id)
        {
            lock (_lockGuard)
            {
                if (!_sessionDictionary.ContainsKey(id))
                {
                    throw new ArgumentException($"Session {id} not found!");
                }

                ComputationGraph session = _sessionDictionary[id];
                SessionStatus status = _sessionStatus[id];

                if (SessionUtilities.SessionCompleted(status.OperationStatus))
                {
                    _logger.LogInformation("Session {0} requested to stop after completion", id);
                }
                else if (SessionUtilities.SessionFaild(status.OperationStatus))
                {
                    foreach(OperationStatus operation in status.OperationStatus.Where(op => op.Status == OperationState.Awaits))
                    {
                        operation.Status = OperationState.Aborted;
                    }
                    _logger.LogError("Session {0} requested to stop but was aborted");
                }
                else
                {
                    foreach (OperationStatus operation in status.OperationStatus.Where(op => op.Status == OperationState.Awaits))
                    {
                        operation.Status = OperationState.Aborted;
                    }
                    _logger.LogInformation("Aborting session {0} by user request", id);
                }
            }
        }

        public SessionStatus GetStatusSession(Guid idSession)
        {
            if (_sessionStatus.ContainsKey(idSession))
            {
                return _sessionStatus[idSession];
            }
            else
            {
                throw new ArgumentException($"Session {idSession} not found!");
            }
        }
    }
}
