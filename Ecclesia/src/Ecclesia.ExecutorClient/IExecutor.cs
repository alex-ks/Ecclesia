using Ecclesia.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesia.ExecutorClient
{
    public interface IExecutor
    {
        string StartSession(SessionStartRequest startRequest);

        SessionStatus GetSessionStatus(string sessionId);

        void AbortSession(string sessionId);
    }
}
