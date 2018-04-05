using Microsoft.EntityFrameworkCore;

namespace Ecclesia.Resolver.DataAccess
{
    public class NpgsqlResolverContext : ResolverContext
    {
        private readonly string _connectionString;

        public NpgsqlResolverContext()
        {
            _connectionString = @"Server=ecclesia.ict.nsc.ru;Port=5432;Database=ecclesia_resolver_db;Username=ecclesia;Password=Str0ngP@ssw0rd;";
        }

        public NpgsqlResolverContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}