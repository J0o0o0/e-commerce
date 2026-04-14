using e_commerce.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using e_commerce.Services.Interfaces;
using e_commerce.Extensions;

namespace e_commerce.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);
            return Ok(products);
        }
        [HttpGet("price")]
        public async Task<IActionResult> GetByPrice(decimal min, decimal max)
        {
            var products = await _productService.GetByPriceRangeAsync(min, max);
            return Ok(products);
        }
        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var userId = User.GetUserId();

            var product = await _productService.CreateAsync(dto, userId);

            return Ok(product);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var userId = User.GetUserId();

            await _productService.UpdateAsync(id, dto, userId);

            return NoContent();
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _productService.DeleteAsync(id, userId);

            return NoContent();
        }
    }
}