using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecclesia.Models
{
    public class SessionStatus
    {
        public long? SessionId { get; set; }

        [JsonRequired]
        public List<OperationStatus> OperationStatus { get; set; }

        [JsonRequired]
        public Dictionary<string, MnemonicsValue> MnemonicsTable { get; set; }

        public DateTime? StartTime { get; set; }
    }
}
