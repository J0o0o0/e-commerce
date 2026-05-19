using System;
using System.Collections.Generic;

namespace e_commerce.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Buyer Buyer { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}