using System;

namespace e_commerce.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; } // For ordering images
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Product Product { get; set; }
    }
}