using e_commerce.Data;
using e_commerce.DTOs.ProductDTOs;
using e_commerce.Models;
using e_commerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace e_commerce.Repositories.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<Product>> GetFilteredAsync(ProductQueryDto query)
        {
            var q = _context.Products
                .Include(p => p.Category)
                .Include(i => i.Images)
                .Include(p => p.Seller)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                q = q.Where(p =>
                    EF.Functions.Like(p.Name, $"%{search}%") ||
                    EF.Functions.Like(p.Description, $"%{search}%") ||
                    EF.Functions.Like(p.Seller.StoreName, $"%{search}%"));

                q = q
                    .OrderByDescending(p =>
                        (EF.Functions.Like(p.Name, search) ? 3 : 0) +
                        (EF.Functions.Like(p.Name, $"{search}%") ? 2 : 0) +
                        (EF.Functions.Like(p.Name, $"%{search}%") ? 1 : 0) + 
                        (EF.Functions.Like(p.Description, $"%{search}%") ? 1 : 0)
                    )
                    .ThenBy(p => p.Id);
            }

            if (query.CategoryId.HasValue)
                q = q.Where(p => p.CategoryId == query.CategoryId);

            if (query.SellerId.HasValue)
                q = q.Where(p => p.SellerId == query.SellerId);

            if (query.MinPrice.HasValue)
                q = q.Where(p => p.Price >= query.MinPrice);

            if (query.MaxPrice.HasValue)
                q = q.Where(p => p.Price <= query.MaxPrice);

            
            q = q.OrderBy(p => p.Id);

            var total = await q.CountAsync();

            var data = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResultDto<Product>
            {
                Data = data,
                Total = total
            };
        }   
        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                
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