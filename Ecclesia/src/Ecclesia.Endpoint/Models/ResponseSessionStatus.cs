using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecclesia.Endpoint.Models
{
    public class ResponseSessionStatus
    {
        public long? SessionId { get; set; }

        [JsonRequired]
        public List<ResponseOperationStatus> OperationStatus { get; set; }

        [JsonRequired]
        public Dictionary<string, Ecclesia.Models.MnemonicsValue> MnemonicsTable { get; set; }

        public DateTime? StartTime { get; set; }
    }
}
