using Ecclesia.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.DataAccessLayer.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.Property(session => session.StartTime)
                .HasColumnType("timestamptz");

            builder.Property(session => session.LastPolling)
                .HasColumnType("timestamptz");

            builder.Ignore(session => session.OriginalGraph);
            builder.Ignore(session => session.OperationsStatus);
            builder.Ignore(session => session.MnemonicsTable);

            builder.Property(session => session.OriginalGraphJson)
                .HasColumnType("jsonb")
                .HasColumnName(nameof(Session.OriginalGraph));
            builder.Property(session => session.OperationsStatusJson)
                .HasColumnType("jsonb")
                .HasColumnName(nameof(Session.OperationsStatus));
            builder.Property(session => session.MnemonicsTableJson)
                .HasColumnType("jsonb")
                .HasColumnName(nameof(Session.MnemonicsTable));
        }
    }
}