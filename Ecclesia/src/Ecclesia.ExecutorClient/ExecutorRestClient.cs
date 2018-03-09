using System;
using System.Collections.Generic;
using System.Text;
using Ecclesia.Models;

namespace Ecclesia.ExecutorClient
{
    public class ExecutorRestClient : IExecutor
    {
        private readonly string _hostname;

        public ExecutorRestClient(string hostname)
        {
            _hostname = hostname;
        }

        public void AbortSession(string sessionId)
        {
            throw new NotImplementedException();
        }

        public SessionStatus GetSessionStatus(string sessionId)
        {
            throw new NotImplementedException();
        }

        public string StartSession(SessionStartRequest startRequest)
        {
            throw new NotImplementedException();
        }
    }
}
