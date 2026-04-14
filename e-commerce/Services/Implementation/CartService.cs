using e_commerce.DTOs;
using e_commerce.DTOs.CartDTOs;
using e_commerce.Models;
using Microsoft.EntityFrameworkCore;
using e_commerce.Repositories.Interfaces;
using e_commerce.Services.Interfaces;


namespace e_commerce.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private async Task<Cart> CreateCartAsync(int buyerId)
        {
            var cart = new Cart
            {
                BuyerId = buyerId,
                Items = new List<CartItem>()
            };
            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
            return cart;
        }
        private async Task<Cart> GetCartObjectAsync(int userId)
        {
            var buyer = await _unitOfWork.Users.GetBuyerByUserIdAsync(userId);
            if (buyer == null)
                throw new Exception("User needs to login");

            var cart = await _unitOfWork.Carts.GetByBuyerIdWithItemsAsync(buyer.Id);

            if (cart == null)
                cart = await CreateCartAsync(buyer.Id);

            return cart;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await GetCartObjectAsync(userId);
            return new CartDto
            {
                Items = cart.Items.Select(i => new CartItemDto
                {
                    ImageUrl = i.Product.Images.FirstOrDefault()?.ImageUrl,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Price = i.Product.Price,
                    Quantity = i.Quantity,
                    PriceAtAddTime = i.PriceAtAddTime
                }).ToList(),
                TotalPrice = cart.Items.Sum(i => i.Quantity * i.PriceAtAddTime),
            };
        }

        public async Task AddToCartAsync(int userId, AddToCartDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            var cart = await GetCartObjectAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                if (existingItem.Quantity + dto.Quantity > product.Stock)
                    throw new Exception("Not enough stock available");
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                if (dto.Quantity > product.Stock)
                    throw new Exception("Not enough stock available");
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = dto.Quantity,
                    PriceAtAddTime = product.Price
                });
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            var cart = await GetCartObjectAsync(userId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new Exception("Product not found in cart");

            cart.Items.Remove(item);
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetCartObjectAsync(userId);
            if (!cart.Items.Any())
                return;

            cart.Items.Clear();
            await _unitOfWork.SaveChangesAsync(); 
        }
    }
}

