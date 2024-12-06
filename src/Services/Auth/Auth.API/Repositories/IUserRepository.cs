using Auth.API.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Security.Models;
using System.Linq.Expressions;

namespace Auth.API.Repositories
{
    public interface IUserRepository
    {
        Task<CoffeeShopUser?> FindUserByConditionAsync(Expression<Func<CoffeeShopUser, bool>> predicate);
        Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId);
        Task<(IdentityResult, CoffeeShopUser?)> GetUserByEmailAsync(string email);
        Task<string> GenerateEmailConfirmationTokenAsync(CoffeeShopUser user);
        Task<string> GeneratePasswordResetTokenAsync(CoffeeShopUser user);
        Task<IdentityResult> ResetPasswordAsync(CoffeeShopUser user, string decodedToken, string newPassword);
        Task<(IdentityResult, string?)> CreateUserAsync(CoffeeShopUser user, string password, Roles role);
        Task<IdentityResult> UpdateUserAsync(CoffeeShopUser user);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> UpdateUserPasswordAsync(CoffeeShopUser user, string newPassword);
        Task<bool> CheckPasswordAsync(CoffeeShopUser user, string password);
        Task<IList<string>> GetRolesAsync(CoffeeShopUser user);
    }
}
