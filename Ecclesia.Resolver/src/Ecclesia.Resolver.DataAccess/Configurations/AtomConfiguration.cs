using Ecclesia.Resolver.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.Resolver.DataAccess.Configurations
{
    public class AtomConfiguration : IEntityTypeConfiguration<Atom>
    {
        public void Configure(EntityTypeBuilder<Atom> builder)
        {
            builder.HasAlternateKey(atom => new { atom.Kind, atom.Name, atom.Version });
            builder.HasOne(atom => atom.Content)
                   .WithOne(ac => ac.Atom)
                   .HasForeignKey<AtomContent>(ac => ac.AtomId);
        }
    }
}