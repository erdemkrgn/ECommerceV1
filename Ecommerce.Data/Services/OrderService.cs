using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateOrderAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Sepet boş. Sipariş oluşturulamaz.");

            var totalPrice = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = totalPrice,
                Status = OrderStatus.Pending,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            _context.CartItems.RemoveRange(cartItems); // Sepeti temizle
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Sipariş bulunamadı.");

            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }
    }
}
