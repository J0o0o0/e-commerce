using e_commerce.DTOs.AuthDTOs;

namespace e_commerce.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterBuyerAsync(RegisterBuyerDto dto);
        Task<AuthResultDto> RegisterSellerAsync(RegisterSellerDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
    }
}