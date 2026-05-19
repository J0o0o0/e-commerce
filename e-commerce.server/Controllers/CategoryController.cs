using e_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService) 
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _categoryService.GetAll();
            return Ok(categorias);
        }
    }
}
