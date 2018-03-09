using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesia.DataAccessLayer
{
    public class PsqlContext : EcclesiaContext
    {
        private readonly string _connectionString;

        public PsqlContext()
        {
            _connectionString = @"Server=ecclesia.ict.nsc.ru;Port=5432;Database=ecclesia_db;Username=ecclesia;Password=Str0ngP@ssw0rd;";
        }

        public PsqlContext(string connectionString)
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
