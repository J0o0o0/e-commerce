using e_commerce.DTOs.OrderDTOs;

namespace e_commerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
        Task<OrderResponseDto> GetOrderByIdAsync(int orderId, int userId);
        Task<IEnumerable<OrderResponseDto>> GetOrderHistoryAsync(int userId);
        Task<OrderResponseDto> CancelOrderAsync(int orderId, int userId);
    }
}
