using Ecclesia.Resolver.Orm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.Resolver.Orm.Configurations
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