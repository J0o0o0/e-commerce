using e_commerce.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public int BuyerId { get; set; }
    public decimal TotalPrice { get; set; }

    // Separate concerns
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public PaymentProvider PaymentProvider { get; set; }
    public string? PaymentIntentId { get; set; }   // Stripe
    public string? PaymobOrderId { get; set; }      // Paymob
    public string ShippingAddress { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Buyer Buyer { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

// Fulfillment status — where is the order physically?
public enum OrderStatus
{
    Pending = 1,        // order placed, waiting to process
    Processing = 2,     // seller is preparing
    Shipped = 3,        // handed to courier
    OutForDelivery = 4, // courier is on the way
    Delivered = 5,      // customer received it
    Cancelled = 6
}

// Payment status — is the order paid?
public enum PaymentStatus
{
    Pending = 1,        // not paid yet
    Paid = 2,           // payment successful
    Failed = 3,         // payment failed
    Refunded = 4        // money returned (on cancel)
}

public enum PaymentProvider
{
    CashOnDelivery = 1,
    Stripe = 2,
    Paymob = 3
}