using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppALZA.API.Models;

namespace WebAppALZA.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbcontext;

        public ProductRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _dbcontext.Products.ToListAsync<Product>();
            return products;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var employee = await _dbcontext.Products.AsNoTracking().Where(empid => empid.Id == id).FirstOrDefaultAsync();
            return employee;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _dbcontext.Entry(product).State = EntityState.Modified;
            await _dbcontext.SaveChangesAsync();
            return product; 
        }
    }
}
