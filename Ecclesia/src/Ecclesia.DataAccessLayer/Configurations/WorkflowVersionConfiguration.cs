using Ecclesia.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.DataAccessLayer.Configurations
{
    public class WorkflowVersionConfiguration : IEntityTypeConfiguration<WorkflowVersion>
    {
        public void Configure(EntityTypeBuilder<WorkflowVersion> builder)
        {
            builder.Property(version => version.CreationDate)
                .HasColumnType("timestamptz");
            
            builder.HasOne(version => version.Workflow)
                .WithMany(wf => wf.Versions)
                .HasForeignKey(version => version.WorkflowId);

            builder.HasKey(version => new { version.WorkflowId, version.VersionName });
        }
    }
}