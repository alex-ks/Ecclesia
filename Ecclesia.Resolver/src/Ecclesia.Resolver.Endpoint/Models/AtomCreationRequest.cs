using System.Collections.Generic;
using Ecclesia.Resolver.Storage.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ecclesia.Resolver.Endpoint.Models
{
    public class AtomCreationRequest
    {
        [JsonRequired]
        public string Kind { get; set; }
        [JsonRequired]
        public string Name { get; set; }
        public string Version { get; set; }
        public string Content { get; set; }
        public JObject Meta { get; set; }
        [JsonRequired]
        public IEnumerable<AtomIdArgument> Dependencies { get; set; }
    }
}