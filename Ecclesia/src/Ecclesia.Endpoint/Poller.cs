using Ecclesia.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesia.Endpoint
{
    public struct SessionPollingTask
    {
        public long SessionId { get; set; }
    }

    public class Poller
    {
        private static readonly TimeSpan PollingDelay = TimeSpan.FromSeconds(1);

        private readonly IServiceProvider _services;
        private readonly IMessageQueue<SessionPollingTask> _queue;

        public Poller(IServiceProvider services, IMessageQueue<SessionPollingTask> queue)
        {
            _services = services;
            _queue = queue;
        }

        public void StartTracking(long id)
        {
            _queue.Push(new SessionPollingTask { SessionId = id }, PollingDelay);
        }
    }
}
