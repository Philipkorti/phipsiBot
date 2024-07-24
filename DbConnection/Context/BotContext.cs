using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConnection.Entity;
using Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace DbConnection.Context
{
    public class BotContext :DbContext
    {
        public DbSet<TimeHelper> TimeHelper { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddJsonFile("config/config.json");
            IConfiguration configuration = configurationBuilder.Build();
            string connectionString = configuration["dbConection"];
            optionsBuilder.UseMySQL(connectionString);
        }

    }
}
