namespace e_commerce.DTOs.CartDTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}