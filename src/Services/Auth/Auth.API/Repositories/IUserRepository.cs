using Auth.API.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Repositories
{
    public interface IUserRepository
    {
        Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId);
        Task<(IdentityResult, string?)> CreateUserAsync(CoffeeShopUser user, string password);
        Task<IdentityResult> UpdateUserAsync(CoffeeShopUser user);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> AddUserToRoleAsync(string userId, Roles role);
        Task<IdentityResult> UpdateUserPasswordAsync(CoffeeShopUser user, string newPassword);
    }
}
