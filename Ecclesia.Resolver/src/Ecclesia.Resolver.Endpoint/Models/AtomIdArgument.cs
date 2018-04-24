using Newtonsoft.Json;

namespace Ecclesia.Resolver.Endpoint.Models
{
    public class AtomIdArgument
    {
        [JsonRequired]
        public string Kind { get; set; }
        [JsonRequired]
        public string Name { get; set; }
        public string Version { get; set; }
    }
}