using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ecclesia.Models;
using Newtonsoft.Json;

namespace Ecclesia.ExecutorClient
{
    public class ExecutorRestClient : IExecutor
    {
        private readonly string _hostname;

        private class ErrorResponse
        {
            public string Error { get; set; }
        }

        public ExecutorRestClient(string hostname)
        {
            _hostname = hostname;
        }

        public async Task AbortSessionAsync(string sessionId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.DeleteAsync($"{_hostname}/api/session/{sessionId}");

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(responseString);
                }
            }
        }

        public async Task<SessionStatus> GetSessionStatusAsync(string sessionId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_hostname}/api/session/{sessionId}");
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<SessionStatus>(responseString);
                }
                else
                {
                    throw new InvalidOperationException(responseString);
                }
            }
        }

        public async Task<string> StartSessionAsync(SessionStartRequest startRequest)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(startRequest), 
                    Encoding.UTF8, 
                    "application/json");
                var response = await client.PostAsync($"{_hostname}/api/session", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeAnonymousType(responseString, new { SessionId = "" }).SessionId;
                }
                else
                {
                    throw new InvalidOperationException(responseString);
                }
            }
        }
    }
}
