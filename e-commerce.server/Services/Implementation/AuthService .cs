using e_commerce.Models;
using Microsoft.AspNetCore.Identity;
using e_commerce.Repositories.Interfaces;
using e_commerce.Services.Interfaces;
using e_commerce.DTOs.AuthDTOs;

namespace e_commerce.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;


        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return _jwtService.GenerateToken(user, roles);
        }

        public async Task<AuthResultDto> RegisterBuyerAsync(RegisterBuyerDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Role = UserRole.Buyer
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Buyer");

            await _unitOfWork.Buyers.AddAsync(new Buyer
            {
                UserId = user.Id,
                ShippingAddress = dto.ShippingAddress,
            });
            await _unitOfWork.SaveChangesAsync();


            var token = await GenerateToken(user);

            return new AuthResultDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = "Buyer"
            };
        }

        public async Task<AuthResultDto> RegisterSellerAsync(RegisterSellerDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = UserRole.Seller
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Seller");

            // create seller profile
            await _unitOfWork.Sellers.AddAsync(new Seller
            {
                UserId = user.Id,
                StoreAddress = dto.StoreAddress,
                StoreName = dto.StoreName,
                Description = dto.Description
            });
            await _unitOfWork.SaveChangesAsync();

            var token = await GenerateToken(user);

            return new AuthResultDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = "Seller"
            };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {


            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                throw new Exception("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user, roles);

            return new AuthResultDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = roles.FirstOrDefault() // Assuming one role per user
            };
        }


    }
}