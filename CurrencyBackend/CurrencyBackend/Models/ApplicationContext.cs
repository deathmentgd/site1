using CurrencySource.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyBackend.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Currency> CurrencyList { get; set; }
        public DbSet<CurrencyTracked> TrackedCurrencyList { get; set; }

        public ApplicationContext()
        {
            Database.SetCommandTimeout(20);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = Startup.ConnectinString;
            /*ServerVersion version = ServerVersion.AutoDetect(connection);
            optionsBuilder.UseMySql(connection, version);*/
            optionsBuilder.UseSqlServer(connection);
        }
    }
}
