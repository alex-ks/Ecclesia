namespace Ecclesia.Resolver.Orm.Models
{
    public class AtomDependency
    {
        public long DependentId { get; set; }
        public Atom Dependent { get; set; }

        public long DependencyId { get; set; }
        public Atom Dependency { get; set; }
    }
}