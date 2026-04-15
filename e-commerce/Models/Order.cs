using System;
using System.Collections.Generic;

namespace e_commerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentProvider PaymentProvider { get; set; }
        public string? PaymentIntentId { get; set; }   //  Stripe
        public string? PaymobOrderId { get; set; }      //  Paymob
        public string ShippingAddress { get; set; } // = ShippingAddress of the buyer at the time of order
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Buyer Buyer { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
    public enum PaymentProvider { Stripe, Paymob }
}