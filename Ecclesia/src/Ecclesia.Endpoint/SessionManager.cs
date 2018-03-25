using Ecclesia.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ecclesia.DataAccessLayer.Models;
using Ecclesia.Models;
using Ecclesia.ExecutorClient;
using Ecclesia.Identity.Models;

namespace Ecclesia.Endpoint
{
    public class SessionManager
    {
        private readonly IServiceProvider _services;

        public SessionManager(IServiceProvider services)
        {
            _services = services;    
        }

        internal async Task<long> StartSessionAsync(ApplicationUser user, SessionStartRequest startRequest)
        {
            using (var context = _services.GetService<EcclesiaContext>())
            {
                var executor = _services.GetService<IExecutor>();
                var poller = _services.GetService<Poller>();

                var executionId = await executor.StartSessionAsync(startRequest);

                var session = new Session
                {
                    StartTime = DateTime.Now,
                    LastPolling = null,
                    MnemonicsTable = startRequest.ComputationGraph.MnemonicsTable,
                    OriginalGraph = startRequest.ComputationGraph,
                    OperationsStatus =
                        startRequest.ComputationGraph.Operations
                            .Select(op => new OperationStatus { Id = op.Id, State = OperationState.Awaits })
                            .ToList(),
                    UserId = user.Id,
                    ExecutionId = executionId,
                    State = SessionState.Running
                };

                await context.Sessions.AddAsync(session);
                await context.SaveChangesAsync();

                poller.StartTracking(session.Id);

                return session.Id;
            }
        }

        internal async Task<object> GetSessionStatusAsync(ApplicationUser user, long id)
        {
            using (var context = _services.GetService<EcclesiaContext>())
            {
                var session = await context.Sessions.FindAsync(id);

                if (session == null || session.UserId != user.Id)
                {
                    throw new KeyNotFoundException();
                }

                return new SessionStatus
                {
                    OperationStatus = session.OperationsStatus,
                    MnemonicsTable = session.MnemonicsTable
                };
            }
        }

        internal IEnumerable<SessionStatus> GetSessions(ApplicationUser applicationUser)
        {
            using (var context = _services.GetService<EcclesiaContext>())
            {
                var query = from session in context.Sessions
                            where session.UserId == applicationUser.Id
                            let status = new SessionStatus
                            {
                                SessionId = session.Id,
                                OperationStatus = session.OperationsStatus,
                                MnemonicsTable = session.MnemonicsTable,
                                StartTime = session.StartTime
                            }
                            orderby status.StartTime descending
                            select status;
                return query.ToList();
            }
        }
    }
}
