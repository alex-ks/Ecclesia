using Microsoft.EntityFrameworkCore;

namespace Ecclesia.Resolver.Orm
{
    public class InMemoryResolverContext : ResolverContext
    {
        private const string InMemoryDatabaseName = "resolver";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase(InMemoryDatabaseName);
        }
    }
}