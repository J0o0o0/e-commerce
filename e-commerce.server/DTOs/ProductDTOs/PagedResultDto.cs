namespace e_commerce.DTOs.ProductDTOs
{
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int Total { get; set; }
    }
}
