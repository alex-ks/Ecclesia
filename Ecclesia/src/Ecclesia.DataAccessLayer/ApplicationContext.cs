using Ecclesia.DataAccessLayer.Configurations;
using Ecclesia.DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecclesia.DataAccessLayer
{
    public abstract class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new SessionConfiguration());
        }
    }
}