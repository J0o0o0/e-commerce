namespace e_commerce.DTOs.ProductDTOs
{
    public class ProductBaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> Images { get; set; } = new();
    }
}