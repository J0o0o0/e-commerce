namespace e_commerce.DTOs.ProductDTOs
{
    public class GetProductDto : ProductBaseDto
    {
        public int Id { get; set; }
        public string SellerName { get; set; }

        public double Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}
