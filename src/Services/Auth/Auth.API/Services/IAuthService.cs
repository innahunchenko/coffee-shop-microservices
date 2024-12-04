using Auth.API.Domain.Dtos;
using Microsoft.AspNetCore.Identity;
using Security.Models;
namespace Auth.API.Services
{
    public interface IAuthService
    {
        Task<(IdentityResult, string?)> RegisterUserAsync(CoffeeShopUserDto registerUserDto, Roles role);
        Task<IdentityResult> LoginUserAsync(string userName, string login);
    }
}
