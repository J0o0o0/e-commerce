using e_commerce.Data;
using e_commerce.Models;
using Microsoft.EntityFrameworkCore;
using e_commerce.Repositories.Interfaces;

namespace e_commerce.Repositories.Implementation
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Cart?> GetByBuyerIdWithItemsAsync(int buyerId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.Buyer.Id == buyerId);
        }
    }
}
