using Ecclesia.Resolver.Orm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.Resolver.Orm.Configurations
{
    public class AtomDependencyConfiguration : IEntityTypeConfiguration<AtomDependency>
    {
        public void Configure(EntityTypeBuilder<AtomDependency> builder)
        {
            builder.HasKey(ac => new { ac.DependentId, ac.DependencyId });
            builder.HasOne(ad => ad.Dependent)
                   .WithMany(atom => atom.Dependencies)
                   .HasForeignKey(ad => ad.DependentId);
            builder.HasOne(ad => ad.Dependency)
                   .WithMany(ad => ad.Dependent)
                   .HasForeignKey(ad => ad.DependencyId);
        }
    }
}