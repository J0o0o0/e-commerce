using Microsoft.AspNetCore.Mvc;
using e_commerce.DTOs.AuthDTOs;
using e_commerce.Services.Interfaces;



namespace e_commerce.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-buyer")]
        public async Task<IActionResult> RegisterBuyer(RegisterBuyerDto dto)
        {
            try
            {
                var result = await _authService.RegisterBuyerAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register-seller")]
        public async Task<IActionResult> RegisterSeller(RegisterSellerDto dto)
        {
            try
            {
                var result = await _authService.RegisterSellerAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

    }
}