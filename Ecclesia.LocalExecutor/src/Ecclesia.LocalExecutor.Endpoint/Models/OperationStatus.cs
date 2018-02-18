using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecclesia.LocalExecutor.Endpoint.Models
{
    public class OperationStatus
    {
        [JsonRequired]
        public int Id { get; set; }

        [JsonRequired]
        public OperationState Status { get; set; }
    }
}
