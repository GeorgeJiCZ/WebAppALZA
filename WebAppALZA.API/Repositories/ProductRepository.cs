using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public async Task<IEnumerable<ProductVM>> GetProductsAsync()
        {             
            var products = await _dbcontext.Products.AsNoTracking().ProjectTo<ProductVM>(Util.mapperProduct).ToListAsync();
            return products;             
        }

        public async Task<IEnumerable<ProductVM>> GetProductsAsync(int? pageSize, int pageIndex)
        {
            try 
            {
                int ps = pageSize ?? Int32.Parse(_configuration.GetSection("DefaultPageSize").Value);                 
                var products = await _dbcontext.Products.AsNoTracking().Skip((pageIndex) * ps).Take(ps).ProjectTo<ProductVM>(Util.mapperProduct).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                SetLog(ex, "An error occurred while pagination of products.");                
                return null;
            }
        }

        public async Task<ProductVM> GetProductAsync(int id)
        {
            var product = await _dbcontext.Products.AsNoTracking().Where(item => item.Id == id).ProjectTo<ProductVM>(Util.mapperProduct).SingleOrDefaultAsync();            
            return product;
        }

        public async Task<ProductVM> UpdateProductAsync(ProductVM product)
        {
            var prod = await _dbcontext.Products.FindAsync(product.Id);

            if (prod == null) return null;

            prod.Name = product.Name;
            prod.ImgPath = product.ImgPath;
            prod.Price = product.Price;
            prod.Description = product.Description;

            try
            {
                await _dbcontext.SaveChangesAsync();
                return Util.GetProductMapper(prod);
            }
            catch (Exception ex)
            {
                SetLog(ex, "An error occurred while updating of product.");                
                return null;
            }
        }        

        public async Task<ProductVM> UpdateProductDescAsync(int id, ProductVM product)
        {
            var prod = await _dbcontext.Products.FindAsync(id);
            if (prod == null) return null;

            prod.Description = product.Description;

            try
            {
                await _dbcontext.SaveChangesAsync();
                return Util.GetProductMapper(prod);
            }
            catch (Exception ex)
            {
                SetLog(ex, "An error occurred while description updating of product.");               
                return null;
            }
        }
        
        public async Task<ProductVM> CreateProductAsync(ProductVM product)
        {
            var prod = new Product
            {
                Name = product.Name,
                ImgPath = product.ImgPath,
                Price = product.Price,
                Description = product.Description
            };

            try
            {
                _dbcontext.Products.Add(prod);
                await _dbcontext.SaveChangesAsync();
                return Util.GetProductMapper(prod);
            }
            catch (Exception ex)
            {
                SetLog(ex, "An error occurred while creating of product.");               
                return null;
            }
        }
        
        public async Task DeleteProductAsync(int id)
        {
            var prod = await _dbcontext.Products.FindAsync(id);
            _dbcontext.Products.Remove(prod);
            await _dbcontext.SaveChangesAsync();
        }
        
        public async Task<bool> ProductExistsAsync(int id) => await _dbcontext.Products.AnyAsync(p => p.Id == id);         

        public void SetLog(Exception ex, string message)
        {
            using var serviceScope = _scopeFactory.CreateScope();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ProductRepository>>();
            logger.LogError(ex, message);
        }
    }
}
