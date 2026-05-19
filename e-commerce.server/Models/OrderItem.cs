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
        public decimal TotalPrice { get; set; } // Calculated as Quantity * PricePerUnit

        public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;
        // Navigation properties
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
    public enum OrderItemStatus
    {
        Pending = 1,        // seller hasn't acted yet
        Approved = 2,       // seller approved
        Processing = 3,     // seller is preparing
        Shipped = 4,        // seller shipped
        Delivered = 5,      // buyer received
        Cancelled = 6       // seller cancelled this item
    }
}