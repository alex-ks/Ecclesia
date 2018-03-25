using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecclesia.Models;
using Newtonsoft.Json;

namespace Ecclesia.Endpoint.Models
{
    public class ResponseOperationStatus
    {
        [JsonRequired]
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonRequired]
        public OperationState State { get; set; }
    }
}
