using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using ECommerce.Core.DTOs;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Tüm sepet işlemleri login kullanıcıya özel
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Token geçerli değil veya kullanıcı ID bilgisi içermiyor.");

            return int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int userId;
            try
            {
                userId = GetUserIdFromToken();
            }
            catch
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı token'ı." });
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            var response = _mapper.Map<List<CartItemDto>>(cartItems);

            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            int userId;
            try
            {
                userId = GetUserIdFromToken();
            }
            catch
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı token'ı." });
            }

            await _cartService.AddToCartAsync(userId, productId, quantity);
            return Ok(new { message = "Ürün sepete eklendi." });
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserIdFromToken();

            try
            {
                await _cartService.RemoveFromCartAsync(userId, productId);
                return Ok(new { message = "Ürün sepetten çıkarıldı." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
