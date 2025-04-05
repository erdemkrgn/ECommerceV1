using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Tüm sepet işlemleri login kullanıcıya özel
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
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
            return Ok(cartItems);
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
            int userId;
            try
            {
                userId = GetUserIdFromToken();
            }
            catch
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı token'ı." });
            }
            await _cartService.RemoveFromCartAsync(userId, productId);
            return Ok(new { message = "Ürün sepetten çıkarıldı." });
        }
    }
}

