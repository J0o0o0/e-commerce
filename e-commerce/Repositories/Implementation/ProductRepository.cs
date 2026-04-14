using e_commerce.Data;
using e_commerce.Models;
using Microsoft.EntityFrameworkCore;
using e_commerce.Repositories.Interfaces;

namespace e_commerce.Repositories.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetBySellerIdAsync(int sellerId)
        {
            return await _context.Products
                .Where(p => p.SellerId == sellerId)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .ToListAsync();
        }


        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .ToListAsync();
        }
    }
}