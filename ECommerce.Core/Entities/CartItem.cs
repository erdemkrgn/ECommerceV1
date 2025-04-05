using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        // Kullanıcıya ait
        public int UserId { get; set; }

        // Eklenen ürün
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigasyon özellikleri
        public User? User { get; set; }
        public Product? Product { get; set; }
    }

}
