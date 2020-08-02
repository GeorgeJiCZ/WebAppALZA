using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public IConfiguration _configuration { get; }

        public ProductRepository(AppDbContext dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _dbcontext.Products.AsNoTracking().ToListAsync<Product>();
            return products;
        }

        public async Task<List<Product>> GetProductsAsync(int? pageSize, int pageIndex)
        {             
            int ps = pageSize ?? Int32.Parse(_configuration.GetSection("DefaultPageSize").Value);
            ps = Math.Abs(ps);
            pageIndex = Math.Abs(pageIndex);

            var products = await _dbcontext.Products.AsNoTracking().Skip((pageIndex) * ps).Take(ps).ToListAsync<Product>();
            return products;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _dbcontext.Products.AsNoTracking().Where(item => item.Id == id).FirstOrDefaultAsync();            
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var prod = await _dbcontext.Products.FindAsync(product.Id);
            prod.Description = product.Description;
            await _dbcontext.SaveChangesAsync();
            return prod; 
        }
    }
}
