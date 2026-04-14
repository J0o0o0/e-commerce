using e_commerce.Models;

namespace e_commerce.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Buyer> Buyers { get; }
        IGenericRepository<Seller> Sellers { get; }
        IGenericRepository<Category> Categories { get; }
        ICartRepository Carts { get; }
        IGenericRepository<CartItem> CartItems { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync();
    }
}