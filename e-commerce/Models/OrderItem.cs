using System;

namespace e_commerce.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; } // Price at time of purchase
        public decimal TotalPrice { get; set; } // Quantity * PricePerUnit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}