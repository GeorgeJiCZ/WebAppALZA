using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppALZA.API.Models;

namespace WebAppALZA.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public ProductRepository(AppDbContext dbcontext, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _dbcontext.Products.AsNoTracking().ToListAsync<Product>();
            return products;
        }

        public async Task<List<Product>> GetProductsAsync(int? pageSize, int pageIndex)
        {
            try 
            {
                int ps = pageSize ?? Int32.Parse(_configuration.GetSection("DefaultPageSize").Value); 
                var products = await _dbcontext.Products.AsNoTracking().Skip((pageIndex) * ps).Take(ps).ToListAsync<Product>();
                return products;
            }
            catch (Exception ex)
            {
                using var serviceScope = _scopeFactory.CreateScope();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ProductRepository>>();
                logger.LogError(ex, "An error occurred while pagination of products.");
                return null;
            }
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _dbcontext.Products.AsNoTracking().Where(item => item.Id == id).SingleOrDefaultAsync();            
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var prod = await _dbcontext.Products.FindAsync(product.Id);
            prod.Description = product.Description;
            try
            {
                await _dbcontext.SaveChangesAsync();
                return prod;
            }
            catch (Exception ex)
            {
                using var serviceScope = _scopeFactory.CreateScope();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ProductRepository>>();
                logger.LogError(ex, "An error occurred while updating of product.");
                return null;
            }
        }
    }
}
