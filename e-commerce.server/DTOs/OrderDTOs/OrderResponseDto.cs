using e_commerce.DTOs.OrderDTOs;

public class OrderResponseDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShippingAddress { get; set; }
    public List<OrderItemResponseDto> Items { get; set; }
    public string? ClientSecret { get; set; }
    public string? PaymentToken { get; set; }
}