using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace e_commerce.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Buyer Buyer { get; set; }
        public Seller Seller { get; set; }
    }

    public enum UserRole
    {
        Buyer = 1,
        Seller = 2
    }

    public class Buyer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public class Seller
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double Rating { get; set; } = 0;

        // Navigation property
        public ApplicationUser User { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}