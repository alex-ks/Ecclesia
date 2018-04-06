using System.Collections.Generic;
using Ecclesia.Resolver.Storage.Models;
using Newtonsoft.Json.Linq;

namespace Ecclesia.Resolver.Endpoint.Models
{
    public class AtomCreationRequest
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Content { get; set; }
        public JObject Meta { get; set; }
        public IEnumerable<AtomId> Dependencies { get; set; }
    }
}