using e_commerce.Models;

namespace e_commerce.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByBuyerIdAsync(int buyerId);


        // Seller methods
        Task<List<Order>> GetSellerOrdersAsync(int sellerId, int page, int pageSize, OrderItemStatus? statusFilter);
        Task<int> GetSellerOrdersCountAsync(int sellerId, OrderStatus? statusFilter);
        Task<Order?> GetSellerOrderDetailAsync(int orderId, int sellerId);
        Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId);
    }
}
