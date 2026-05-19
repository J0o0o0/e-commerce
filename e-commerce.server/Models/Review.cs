using System;
using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int BuyerId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Product Product { get; set; }
        public Buyer Buyer { get; set; }
    }
}