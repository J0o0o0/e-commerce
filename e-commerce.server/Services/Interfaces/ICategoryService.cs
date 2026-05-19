using e_commerce.Models;

namespace e_commerce.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
    }
}
