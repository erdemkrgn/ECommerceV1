using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(int userId);
        Task<List<Order>> GetOrdersByUserAsync(int userId);
        Task<List<Order>> GetAllOrdersAsync(); // Admin için
        Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}
