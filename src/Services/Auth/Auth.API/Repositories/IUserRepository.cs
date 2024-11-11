using Auth.API.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Auth.API.Repositories
{
    public interface IUserRepository
    {
        Task<CoffeeShopUser?> FindUserByConditionAsync(Expression<Func<CoffeeShopUser, bool>> predicate);
        Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId);
        Task<(IdentityResult, string?)> CreateUserAsync(CoffeeShopUser user, string password);
        Task<IdentityResult> UpdateUserAsync(CoffeeShopUser user);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> AddUserToRoleAsync(string userId, Roles role);
        Task<IdentityResult> UpdateUserPasswordAsync(CoffeeShopUser user, string newPassword);
        Task<bool> CheckPasswordAsync(CoffeeShopUser user, string password);
        Task<IList<string>> GetRolesAsync(CoffeeShopUser user);
    }
}
