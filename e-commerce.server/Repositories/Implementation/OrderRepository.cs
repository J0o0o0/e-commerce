using e_commerce.Data;
using e_commerce.Models;
using e_commerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace e_commerce.Repositories.Implementation
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Get single order with all details
        public async Task<Order?> GetOrderWithItemsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        // Get all orders for a buyer (history)
        public async Task<IEnumerable<Order>> GetOrdersByBuyerIdAsync(int buyerId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
