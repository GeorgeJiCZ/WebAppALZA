using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppALZA.API.Models;
using WebAppALZA.API.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppALZA.API.Controllers
{
    //[Route("api/[controller]")]
    // [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            var products = await _productRepository.GetProductsAsync();
            if (products == null || products.Count == 0) return NotFound();

            return Ok(products);
        }

        [HttpGet, MapToApiVersion("2.0")]
        public async Task<ActionResult> GetAllAsync(int? pageSize, int pageIndex = 0)
        {
            var products = await _productRepository.GetProductsAsync(pageSize, pageIndex);
            if (products == null || products.Count == 0) return NotFound();

            return Ok(products);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetProductAsync(id);
            if (product == null) return NotFound(); 
            
            return Ok(product);
        }

        // PUT api/<ProductController>/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid || id != product.Id) return BadRequest();
            
            if (await _productRepository.GetProductAsync(id) == null) return NotFound();

            await _productRepository.UpdateProductAsync(product);        
            
            return NoContent();
        }         
    }
}
