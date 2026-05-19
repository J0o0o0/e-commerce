using e_commerce.Models;
using e_commerce.DTOs.CartDTOs;

namespace e_commerce.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);

        Task AddToCartAsync(int userId, AddToCartDto dto);

        Task RemoveFromCartAsync(int userId, int productId);

        Task ClearCartAsync(int userId);
    }
}