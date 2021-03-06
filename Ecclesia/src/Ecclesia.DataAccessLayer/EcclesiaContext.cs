using Ecclesia.DataAccessLayer.Configurations;
using Ecclesia.DataAccessLayer.Models;
using Ecclesia.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecclesia.DataAccessLayer
{
    public abstract class EcclesiaContext : ApplicationContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowVersion> WorkflowVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new WorkflowConfiguration());
            builder.ApplyConfiguration(new WorkflowVersionConfiguration());
        }
    }
}
