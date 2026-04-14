using e_commerce.Models;

namespace e_commerce.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();
        Task<Product?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetBySellerIdAsync(int sellerId);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}