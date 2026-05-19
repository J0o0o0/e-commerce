using System;

namespace e_commerce.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtAddTime { get; set; } // Store price when added (in case product price changes)
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}