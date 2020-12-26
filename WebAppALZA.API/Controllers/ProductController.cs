using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAppALZA.API.Models;
using WebAppALZA.API.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppALZA.API.Controllers
{     
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetProductsAsync()
        {
            var products = await _productRepository.GetProductsAsync();
            if (products == null || products.Count() == 0) return NotFound();

            return Ok(products);
        }

        /*
         * set routing  parametr "Page"
         * NotSupportedException: 
         * Conflicting method/path combination "GET api/v{version}/Product" for actions - GetAllAsync, GetPageAsync. 
         * Actions require a unique method/path combination for Swagger/OpenAPI 3.0. 
         * Use ConflictingActionsResolver as a workaround
         */
        [AllowAnonymous]
        [HttpGet("page"), MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2")]
        public async Task<ActionResult> GetPageAsync(int? pageSize, int pageIndex = 0)
        {
            var products = await _productRepository.GetProductsAsync(pageSize, pageIndex);
            if (products == null || products.Count() == 0) return NotFound();

            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductAsync(int id)
        {
            var product = await _productRepository.GetProductAsync(id);
            if (product == null) return NotFound(); 
            
            return Ok(product);
        }

        //[Authorize(Roles = "Admin, Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProductVM product)
        {
            if (id != product.Id) return BadRequest();

            if (! await _productRepository.ProductExistsAsync(id)) return NotFound();

            await _productRepository.UpdateProductAsync(product);          
            return NoContent();
        }

        //[Authorize(Roles = "Admin, Manager")]
        [HttpPatch("{id}"), MapToApiVersion("3.0")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<IActionResult> UpdateDescAsync(int id, JsonPatchDocument<ProductVM> product)
        {
            var prod = await _productRepository.GetProductAsync(id);
            if (prod == null) return NotFound();

            product.ApplyTo(prod);            
            return Ok(await _productRepository.UpdateProductDescAsync(id, prod));
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost, MapToApiVersion("3.0")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<IActionResult> CreateAsync(ProductVM product)
        {
            if (product.Id != 0) return BadRequest();

            product = await _productRepository.CreateProductAsync(product);

            return CreatedAtAction(nameof(GetProductAsync), new { version = "3", id = product.Id }, product);
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}"), MapToApiVersion("3.0")]
        [ApiExplorerSettings(GroupName = "v3")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (! await _productRepository.ProductExistsAsync(id)) return NotFound();

            await _productRepository.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
