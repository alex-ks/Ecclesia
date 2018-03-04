using System;
using Newtonsoft.Json;

namespace Ecclesia.Models
{
    public class SessionStartRequest
    {
        [JsonRequired]
        public ComputationGraph ComputationGraph { get; set; }

        public DateTime Deadline { get; set; }

        public decimal Budget { get; set; }
    }
}