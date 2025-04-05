using AutoMapper;
using ECommerce.Core.DTOs;
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
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            var response = _mapper.Map<List<ProductResponseDto>>(products);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Ürün bulunamadı." });

            var response = _mapper.Map<ProductResponseDto>(product);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _productService.AddAsync(product);
            return Ok(new { message = "Ürün başarıyla eklendi." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            var existing = await _productService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Güncellenecek ürün bulunamadı." });

            _mapper.Map(dto, existing);
            await _productService.UpdateAsync(existing);

            return Ok(new { message = "Ürün başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new { message = "Ürün başarıyla silindi." });
        }
    }
}
