using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Security.Models;
namespace Auth.API.Services
{
    public interface IAuthService
    {
        Task<(IdentityResult, string?)> RegisterUserAsync(CoffeeShopUserDto registerUserDto, Roles role);
        Task<IdentityResult> LoginUserAsync(string userName, string login);
        Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId);
        Task<(IdentityResult, CoffeeShopUser?)> GetUserByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(CoffeeShopUser user);
        Task<IdentityResult> ResetPasswordAsync(CoffeeShopUser user, string token, string newPassword);
    }
}
