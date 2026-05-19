using e_commerce.Models;

namespace e_commerce.Repositories.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetByBuyerIdWithItemsAsync(int buyerId);
    }
}