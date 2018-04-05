using Ecclesia.Resolver.DataAccess.Configurations;
using Ecclesia.Resolver.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecclesia.Resolver.DataAccess
{
    public abstract class ResolverContext : DbContext
    {
        //public DbSet<Session> Sessions { get; set; }
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