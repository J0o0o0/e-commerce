using e_commerce.Models;

namespace e_commerce.DTOs.OrderDTOs
{
    // Single order as seen by a seller
    public class SellerOrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public decimal MyItemsTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalItemsInOrder { get; set; }
        public int MyItemsCount { get; set; }

        // Buyer info
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string ShippingAddress { get; set; }

        // Only this seller's items
        public List<SellerOrderItemDto> MyItems { get; set; }
    }

    // Single item in seller's view
    public class SellerOrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderItemStatus Status { get; set; }
    }

    // DTO for updating item status
    public class UpdateItemStatusDto
    {
        public OrderItemStatus NewStatus { get; set; }
    }

    // Paginated response
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}