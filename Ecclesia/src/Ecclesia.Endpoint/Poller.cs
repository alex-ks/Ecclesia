using Ecclesia.DataAccessLayer;
using Ecclesia.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ecclesia.ExecutorClient;
using Ecclesia.Models;
using Ecclesia.DataAccessLayer.Models;

namespace Ecclesia.Endpoint
{
    public struct SessionPollingTask
    {
        public long SessionId { get; set; }
    }

    public class Poller : IDisposable
    {
        private static readonly TimeSpan PollingDelay = TimeSpan.FromSeconds(1);

        private readonly IServiceProvider _services;
        private readonly IMessageQueue<SessionPollingTask> _queue;

        public Poller(IServiceProvider services, IMessageQueue<SessionPollingTask> queue)
        {
            _services = services;
            _queue = queue;
            _queue.MessageReceived += PollSession;
        }

        public void StartTracking(long id)
        {
            _queue.Push(new SessionPollingTask { SessionId = id }, PollingDelay);
        }

        public async Task PollSession(SessionPollingTask pollingTask)
        {
            var sessionId = pollingTask.SessionId;

            using (var context = _services.GetService<EcclesiaContext>())
            {
                var session = await context.Sessions.FindAsync(sessionId);
                var executor = _services.GetService<IExecutor>();

                var status = executor.GetSessionStatus(session.ExecutionId);
                session.OperationsStatus = status.OperationStatus;
                session.MnemonicsTable = status.MnemonicsTable;
                session.LastPolling = DateTime.Now;

                if (status.OperationStatus.All(s => s.State == OperationState.Completed))
                    session.State = SessionState.Complete;
                else if (status.OperationStatus.Any(s => s.State == OperationState.Failed))
                    session.State = SessionState.Failed;
                else if (status.OperationStatus.Any(s => s.State == OperationState.Aborted))
                    session.State = SessionState.Aborted;

                context.Sessions.Update(session);

                if (session.State == SessionState.Running)
                    _queue.Push(new SessionPollingTask { SessionId = sessionId }, PollingDelay);

                await context.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            _queue.MessageReceived -= PollSession;
        }
    }
}
