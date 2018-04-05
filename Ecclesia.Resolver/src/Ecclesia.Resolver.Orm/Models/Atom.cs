using System.Collections.Generic;

namespace Ecclesia.Resolver.Orm.Models
{
    public class Atom
    {
        public long Id { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public AtomContent Content { get; set; }
        public List<AtomDependency> Dependencies { get; set; }
        public List<AtomDependency> Dependent { get; set; }
    }
}