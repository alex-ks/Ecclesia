using System.Collections.Generic;
using Ecclesia.Resolver.Storage.Models;

namespace Ecclesia.Resolver.Endpoint.Models
{
    public class AtomHeader
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public IEnumerable<AtomId> Dependencies { get; set; }
    }
}