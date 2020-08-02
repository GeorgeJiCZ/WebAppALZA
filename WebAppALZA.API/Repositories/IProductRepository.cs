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
        Task<List<Product>> GetProductsAsync();

        /// <summary>          
        /// Get one product from database by id.
        /// </summary>
        Task<Product> GetProductAsync(int id);

        /// <summary>          
        /// Update one product.
        /// </summary>
        //Task UpdateProductAsync(int id);

        Task<Product> UpdateProductAsync(Product product);
    }
}
