using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecclesia.Models
{
    public class SessionStatus
    {
        [JsonRequired]
        public List<OperationStatus> OperationStatus { get; set; }

        [JsonRequired]
        public Dictionary<string, MnemonicsValue> MnemonicsTable { get; set; }
    }
}
