using e_commerce.Data;
using e_commerce.Models;
using e_commerce.Repositories.Interfaces;


namespace e_commerce.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IProductRepository Products { get; }
        public IUserRepository Users { get; }
        public IGenericRepository<Buyer> Buyers { get; }
        public IGenericRepository<Seller> Sellers { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<CartItem> CartItems { get; }
        public ICartRepository Carts { get; }


        public UnitOfWork(
            ApplicationDbContext context,
            IProductRepository productRepo,
            IUserRepository usersRepo,
            IGenericRepository<Buyer> buyersRepo,
            IGenericRepository<Seller> sellersRepo,
            IGenericRepository<Category> categoriesRepo,
            IGenericRepository<CartItem> cartItems,
            ICartRepository carts
            )
        {
            _context = context;

            Products = productRepo;
            Users = usersRepo;
            Buyers = buyersRepo;
            Sellers = sellersRepo;
            Categories = categoriesRepo;
            CartItems = cartItems;
            Carts = carts;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}