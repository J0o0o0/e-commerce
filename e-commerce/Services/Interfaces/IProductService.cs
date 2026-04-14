using e_commerce.DTOs.ProductDTOs;


namespace e_commerce.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<GetProductDto>> GetAllAsync();
        Task<GetProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<GetProductDto>> GetBySellerAsync(int sellerId);
        Task<IEnumerable<GetProductDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<GetProductDto>> GetByCategoryAsync(int categoryId);
        Task<GetProductDto> CreateAsync(CreateProductDto dto, int userId);
        Task UpdateAsync(int id, UpdateProductDto dto, int userId);
        Task DeleteAsync(int id, int userId);
    }
}