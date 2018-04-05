using Ecclesia.Resolver.Orm.Configurations;
using Ecclesia.Resolver.Orm.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecclesia.Resolver.Orm
{
    public abstract class ResolverContext : DbContext
    {
        public DbSet<Atom> Atoms { get; set; }
        public DbSet<AtomContent> AtomContents { get; set; }
        public DbSet<AtomDependency> AtomDependencies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AtomConfiguration());
            builder.ApplyConfiguration(new AtomContentConfiguration());
            builder.ApplyConfiguration(new AtomDependencyConfiguration());
        }
    }
}