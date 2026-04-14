using e_commerce.Models;

namespace e_commerce.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserWithDetailsAsync(int id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<Buyer?> GetBuyerByUserIdAsync(int userId);
        Task<Seller?> GetSellerByUserIdAsync(int userId);

    }
}