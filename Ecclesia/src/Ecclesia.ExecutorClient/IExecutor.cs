using Ecclesia.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ecclesia.ExecutorClient
{
    public interface IExecutor
    {
        Task<string> StartSessionAsync(SessionStartRequest startRequest);

        Task<SessionStatus> GetSessionStatusAsync(string sessionId);

        Task AbortSessionAsync(string sessionId);
    }
}
