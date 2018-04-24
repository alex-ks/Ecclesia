using Ecclesia.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecclesia.DataAccessLayer.Configurations
{
    public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.HasAlternateKey(workflow => workflow.AtomName);
            builder.HasAlternateKey(workflow => new { workflow.UserId, workflow.FriendlyName });
        }
    }
}