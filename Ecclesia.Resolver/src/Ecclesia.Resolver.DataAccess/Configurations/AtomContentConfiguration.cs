using Ecclesia.Resolver.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.Resolver.DataAccess.Configurations
{
    public class AtomContentConfiguration : IEntityTypeConfiguration<AtomContent>
    {
        public void Configure(EntityTypeBuilder<AtomContent> builder)
        {
            builder.HasKey(ac => ac.AtomId);
            builder.Property(ac => ac.Content)
                   .HasColumnType("bytea");
        }
    }
}