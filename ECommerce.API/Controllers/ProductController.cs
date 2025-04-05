using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Ürün bulunamadı." });

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Product product)
        {
            await _productService.AddAsync(product);
            return Ok(new { message = "Ürün başarıyla eklendi." });
        }

        // PUT: api/Product
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Product product)
        {
            await _productService.UpdateAsync(product);
            return Ok(new { message = "Ürün başarıyla güncellendi." });
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new { message = "Ürün başarıyla silindi." });
        }
    }
}
