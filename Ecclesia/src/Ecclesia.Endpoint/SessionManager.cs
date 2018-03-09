using Ecclesia.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ecclesia.DataAccessLayer.Models;
using Ecclesia.Models;

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
            using (var context = _services.GetService<ApplicationContext>())
            {
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
                    State = SessionState.Running
                };

                await context.Sessions.AddAsync(session);

                // TODO: call executor
                // TODO: start polling

                await context.SaveChangesAsync();

                return session.Id;
            }
        }

        internal async Task<object> GetSessionStatusAsync(ApplicationUser user, long id)
        {
            using (var context = _services.GetService<ApplicationContext>())
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
            using (var context = _services.GetService<ApplicationContext>())
            {
                return from session in context.Sessions
                       where session.UserId == applicationUser.Id
                       select new SessionStatus
                       {
                           OperationStatus = session.OperationsStatus,
                           MnemonicsTable = session.MnemonicsTable
                       };
            }
        }
    }
}
