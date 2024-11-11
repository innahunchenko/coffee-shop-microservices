using Auth.API.Domain.Dtos;
using Microsoft.AspNetCore.Identity;
namespace Auth.API.Services
{
    public interface IAuthService
    {
        Task<(IdentityResult, string?)> RegisterUserAsync(CoffeeShopUserDto registerUserDto);
        Task<IdentityResult> AddUserToUserRoleAsync(string userId);
        Task<IdentityResult> LoginUserAsync(string userName, string login);
    }
}
