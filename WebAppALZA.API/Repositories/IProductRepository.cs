using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppALZA.API.Models;

namespace WebAppALZA.API.Repositories
{
    public interface IProductRepository
    {
        /// <summary>          
        /// Get all products from database.
        /// </summary>
        Task<IEnumerable<ProductVM>> GetProductsAsync();

        /// <summary>          
        /// Get products from database for pagination.
        /// </summary>
        Task<IEnumerable<ProductVM>> GetProductsAsync(int? pageSize, int pageIndex);

        /// <summary>          
        /// Get one product from database by id.
        /// </summary>
        Task<ProductVM> GetProductAsync(int id);

        /// <summary>          
        /// Update one product.
        /// </summary>
        //Task UpdateProductAsync(int id);

        Task<ProductVM> UpdateProductAsync(ProductVM product);

        /// <summary>          
        /// Update one product.
        /// </summary>
        Task<ProductVM> UpdateProductDescAsync(int id, ProductVM product);

        /// <summary>          
        /// Create product.
        /// </summary>
        Task<ProductVM> CreateProductAsync(ProductVM product);

        /// <summary>          
        /// Delete product.
        /// </summary>
        Task DeleteProductAsync(int id);

        /// <summary>          
        /// If exist product
        /// </summary>
        Task<bool> ProductExistsAsync(int id);
        
        
    }
}
