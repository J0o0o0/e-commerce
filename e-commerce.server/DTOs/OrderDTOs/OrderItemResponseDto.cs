namespace e_commerce.DTOs.OrderDTOs
{
    public class OrderItemResponseDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductImageUrl { get; set; }
    }
}
