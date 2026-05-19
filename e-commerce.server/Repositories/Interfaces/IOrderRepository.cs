using e_commerce.Models;

namespace e_commerce.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByBuyerIdAsync(int buyerId);
    }
}
