using e_commerce.Data;
using e_commerce.Models;
using Microsoft.EntityFrameworkCore;
using e_commerce.Repositories.Interfaces;


namespace e_commerce.Repositories.Implementation
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetUserWithDetailsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Buyer)
                    .ThenInclude(b => b.Orders)
                .Include(u => u.Seller)
                    .ThenInclude(s => s.Products)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<Buyer?> GetBuyerByUserIdAsync(int userId)
        {
            return await _context.Buyers
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }
        public async Task<Seller?> GetSellerByUserIdAsync(int userId)
        {
            return await _context.Sellers
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}