using Ecclesia.DataAccessLayer.Configurations;
using Ecclesia.DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecclesia.DataAccessLayer
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        private readonly string _connectionString;

        public DbSet<Session> Sessions { get; set; }

        public ApplicationContext()
        {
            _connectionString = @"Server=ecclesia.ict.nsc.ru;Port=5432;Database=ecclesia_db;Username=ecclesia;Password=Str0ngP@ssw0rd;";
        }

        public ApplicationContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new SessionConfiguration());
        }
    }
}