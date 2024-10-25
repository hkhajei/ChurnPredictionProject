using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.AccessControl;
using ChurnPredictionProject.Models.Entity;
using ChurnPredictionProject.Models.Service;

namespace ChurnPredictionProject.Data
{
    public class MyDbContext : DbContext
    {

        public DbSet<Customer> Customers { get; set; }
        public DbSet<ChurnPrediction> ChurnPredictions { get; set; }        
        //public DbSet<ForecastResult> ForecastResults { get; set; }
        public MyDbContext(DbContextOptions options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This would only be necessary if you're not passing options via DI
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                          .AddJsonFile("appsettings.json")
                                          .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure();
                });
            }
        }
    }
}
