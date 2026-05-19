using e_commerce.DTOs.OrderDTOs;

namespace e_commerce.Services.Interfaces
{
    public interface IOrderService
    {
        // Buyer methods
        Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
        Task<OrderResponseDto> GetOrderByIdAsync(int orderId, int userId);
        Task<IEnumerable<OrderResponseDto>> GetOrderHistoryAsync(int userId);
        Task<OrderResponseDto> CancelOrderAsync(int orderId, int userId);


        // Seller methods
        Task<PaginatedResponse<SellerOrderResponseDto>> GetSellerOrdersAsync(int userId, int page, int pageSize, int? statusFilter);
        Task<SellerOrderResponseDto> GetSellerOrderDetailAsync(int orderId, int userId);
        Task<SellerOrderItemDto> UpdateItemStatusAsync(int orderItemId, int userId, UpdateItemStatusDto dto);
    }
}
