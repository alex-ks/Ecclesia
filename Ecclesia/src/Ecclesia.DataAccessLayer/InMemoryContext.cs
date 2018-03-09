using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesia.DataAccessLayer
{
    public class InMemoryContext : ApplicationContext
    {
        private const string InMemoryDatabaseName = "ecclesia";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase(InMemoryDatabaseName);
        }
    }
}
