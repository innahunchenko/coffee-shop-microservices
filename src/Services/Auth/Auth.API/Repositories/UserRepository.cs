using Auth.API.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Security.Models;
using System.Linq.Expressions;

namespace Auth.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<CoffeeShopUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserRepository(
            UserManager<CoffeeShopUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<CoffeeShopUser?> FindUserByConditionAsync(Expression<Func<CoffeeShopUser, bool>> predicate)
        {
            return await userManager.Users.FirstOrDefaultAsync(predicate);
        }

        public async Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user == null
                ? (IdentityResult.Failed(new IdentityError { Description = $"User with Id {userId} not found." }), null)
                : (IdentityResult.Success, user);
        }
        
        public async Task<(IdentityResult, string?)> CreateUserAsync(CoffeeShopUser user, string password, Roles role)
        {
            try
            {
                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded || string.IsNullOrEmpty(user.Id))
                {
                    return (result, null);
                }

                var roleName = role.ToString().ToUpper();

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                await userManager.AddToRoleAsync(user, roleName);
                return (result, result.Succeeded ? user.Id : null);
            }
            catch
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "UnknownError",
                    Description = "An unexpected error occurred while creating the user."
                }), null);
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(CoffeeShopUser user)
        {
            var (result, existingUser) = await GetUserByIdAsync(user.Id);

            if (!result.Succeeded || existingUser == null)
                return result;

            existingUser.Email = user.Email;
            existingUser.UserName = user.UserName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.UserName = user.UserName;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.DateOfBirth = user.DateOfBirth;

            return await userManager.UpdateAsync(existingUser);
        }

        public async Task<IdentityResult> UpdateUserPasswordAsync(CoffeeShopUser user, string newPassword)
        {
            var removePasswordResult = await userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
                return removePasswordResult;
            return await userManager.AddPasswordAsync(user, newPassword);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var (result, existingUser) = await GetUserByIdAsync(userId);
            return !result.Succeeded || existingUser == null
                ? result
                : await userManager.DeleteAsync(existingUser);
        }

        public async Task<bool> CheckPasswordAsync(CoffeeShopUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IList<string>> GetRolesAsync(CoffeeShopUser user)
        {
            return await userManager.GetRolesAsync(user);
        }
    }
}
