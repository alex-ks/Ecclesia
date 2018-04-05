using Ecclesia.Resolver.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.Resolver.DataAccess.Configurations
{
    public class AtomDependencyConfiguration : IEntityTypeConfiguration<AtomDependency>
    {
        public void Configure(EntityTypeBuilder<AtomDependency> builder)
        {
            builder.HasKey(ac => new { ac.AtomId, ac.DependentId });
            builder.HasOne(ad => ad.Atom)
                   .WithMany(atom => atom.Dependencies)
                   .HasForeignKey(ad => ad.AtomId);
            builder.HasOne(ad => ad.Dependency)
                   .WithMany(ad => ad.Dependent)
                   .HasForeignKey(ad => ad.DependentId);
        }
    }
}