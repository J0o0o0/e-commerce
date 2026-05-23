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
                    .ThenInclude(b => b.User)
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

        // === SELLER METHODS ===

        // Get orders that contain this seller's products (paginated)
        public async Task<List<Order>> GetSellerOrdersAsync(int sellerId, int page, int pageSize, OrderItemStatus? statusFilter)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Buyer)
                    .ThenInclude(b => b.User)
                .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId));

            if (statusFilter.HasValue)
            {
                query = query
                    .Where(o => o.Items
                        .Where(i => i.Product.SellerId == sellerId)
                            .Any(i => i.Status == statusFilter));
            }

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Count total seller orders for pagination
        public async Task<int> GetSellerOrdersCountAsync(int sellerId, OrderStatus? statusFilter)
        {
            var query = _context.Orders
                .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId));

            if (statusFilter.HasValue)
            {
                query = query.Where(o => o.Status == statusFilter.Value);
            }

            return await query.CountAsync();
        }

        // Get specific order detail from seller's perspective
        public async Task<Order?> GetSellerOrderDetailAsync(int orderId, int sellerId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Buyer)
                    .ThenInclude(b => b.User)
                .Where(o => o.Id == orderId)
                .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId))
                .FirstOrDefaultAsync();
        }

        // Get a single order item
        public async Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId)
        {
            return await _context.OrderItems
                .Include(i => i.Product)
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.Id == orderItemId);
        }
    }
}