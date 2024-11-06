using Auth.API.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<CoffeeShopUser> GetAllUsersAsync();
        Task<CoffeeShopUser?> GetUserByIdAsync(string userId);
        Task<(IdentityResult, string?)> CreateUserAsync(CoffeeShopUser user, string password);
        Task<IdentityResult> UpdateUserAsync(CoffeeShopUser user);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> AddUserToRoleAsync(string userId, Roles role);
        Task<IEnumerable<string>> GetUserRolesAsync(CoffeeShopUser user);
        Task<bool> CheckPasswordAsync(CoffeeShopUser user, string password);
        Task<IdentityResult> UpdateUserPasswordAsync(CoffeeShopUser user, string newPassword);
    }
}
