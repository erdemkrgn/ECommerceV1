using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;
using ECommerce.Core.Entities;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Token geçerli değil.");
            return int.Parse(userIdClaim.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            int userId = GetUserIdFromToken();

            try
            {
                await _orderService.CreateOrderAsync(userId);
                return Ok(new { message = "Sipariş başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            int userId = GetUserIdFromToken();
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            var result = _mapper.Map<List<OrderResponseDto>>(orders);
            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var result = _mapper.Map<List<OrderResponseDto>>(orders);
            return Ok(result);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            if (!Enum.TryParse<OrderStatus>(newStatus, out var status))
                return BadRequest(new { message = "Geçersiz sipariş durumu." });

            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, status);
                return Ok(new { message = "Sipariş durumu güncellendi." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
