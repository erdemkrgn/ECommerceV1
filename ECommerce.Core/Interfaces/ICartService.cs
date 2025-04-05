using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartItemsAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int userId, int productId);
    }
}

