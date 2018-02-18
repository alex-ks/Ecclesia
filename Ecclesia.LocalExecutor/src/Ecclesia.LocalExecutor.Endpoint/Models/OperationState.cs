using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ecclesia.LocalExecutor.Endpoint.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OperationState
    {
        Awaits,
        Running,
        Completed,
        Aborted,
        Failed
    }
}
