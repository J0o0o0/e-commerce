using e_commerce.DTOs.ProductDTOs;
using e_commerce.Models;
using Microsoft.EntityFrameworkCore;
using e_commerce.Repositories.Interfaces;
using e_commerce.Services.Interfaces;

namespace e_commerce.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GetProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithDetailsAsync();

            return products.Select(MapToDto);
        }
        public async Task<GetProductDto> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id);
            if (product == null)
                throw new Exception("Product not found");
            return MapToDto(product);
        }
        public async Task<IEnumerable<GetProductDto>> GetBySellerAsync(int sellerId)
        {
            var products = await _unitOfWork.Products.GetBySellerIdAsync(sellerId);
            return products.Select(MapToDto);
        }
        public async Task<IEnumerable<GetProductDto>> GetByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetByCategoryIdAsync(categoryId);

            return products.Select(MapToDto);
        }
        public async Task<IEnumerable<GetProductDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var products = await _unitOfWork.Products.GetByPriceRangeAsync(minPrice, maxPrice);

            return products.Select(MapToDto);
        }




        public async Task<GetProductDto> CreateAsync(CreateProductDto dto, int userId)
        {
            // get seller from userId
            var seller = await _unitOfWork.Sellers
                .GetByIdAsync(userId);

            if (seller == null)
                throw new Exception("you cant create a product without a seller account");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = (await _unitOfWork.Categories.GetByNameAsync(dto.CategoryName)).Id,
                SellerId = seller.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(product);

            // images
            foreach (var url in dto.Images)
            {
                product.Images.Add(new ProductImage
                {
                    ImageUrl = url
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task UpdateAsync(int id, UpdateProductDto dto, int userId)
        {
            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id);

            if (product == null)
                throw new Exception("Product not found");

            var seller = await _unitOfWork.Sellers
                .GetByIdAsync(userId);

            if (seller == null || product.SellerId != seller.Id)
                throw new Exception("Unauthorized");

            // update basic fields
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            // 🔥 Replace images
            product.Images.Clear();

            foreach (var url in dto.Images)
            {
                product.Images.Add(new ProductImage
                {
                    ImageUrl = url,
                    ProductId = product.Id
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                throw new Exception("Product not found");

            var seller = await _unitOfWork.Sellers
                .GetByIdAsync(userId);

            if (seller == null || product.SellerId != seller.Id)
                throw new Exception("Unauthorized");

            _unitOfWork.Products.Delete(product);

            await _unitOfWork.SaveChangesAsync();
        }







        private GetProductDto MapToDto(Product p)
        {
            return new GetProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                SellerName = p.Seller?.StoreName,
                Rating = p.Rating,
                ReviewCount = p.ReviewCount,
                Images = p.Images.Select(i => i.ImageUrl).ToList()
            };
        }
    }
}