namespace Ecclesia.Resolver.DataAccess.Models
{
    public class AtomDependency
    {
        public long AtomId { get; set; }
        public Atom Atom { get; set; }

        public long DependentId { get; set; }
        public Atom Dependency { get; set; }
    }
}