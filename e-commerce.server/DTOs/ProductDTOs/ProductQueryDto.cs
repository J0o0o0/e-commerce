namespace e_commerce.DTOs.ProductDTOs
{
    public class ProductQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public int? SellerId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
