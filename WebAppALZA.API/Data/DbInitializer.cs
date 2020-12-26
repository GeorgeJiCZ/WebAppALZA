using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WebAppALZA.API.Models;

namespace WebAppALZA.API.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IConfiguration _configuration;

        public DbInitializer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        public void Initialize()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            try
            {                
                if (context.Database.GetPendingMigrations().Any())  context.Database.Migrate();                  
            }
            catch (Exception ex)
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
                logger.LogError(ex, "An error occurred while initialize the database.");
            }
        }

        public void SeedData()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            try
            {
                context.Database.EnsureCreated();
                if (!context.Products.Any())
                {
                    context.Products.AddRange(SetData());
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            } 
        }

        private List<Product> SetData()
        {            
            var json = File.ReadAllText(_configuration.GetSection("InitFilePath").Value); 
            return JsonSerializer.Deserialize<List<Product>>(json);
        }
    }
}
