using Auth.API.Domain.Models;
using LanguageExt.Pipes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<CoffeeShopUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserRepository(UserManager<CoffeeShopUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (IdentityResult.Failed(new IdentityError() { Description = $"There is no user with Id {userId}" }), null);
            }

            return (IdentityResult.Success, user);
        }

        public async Task<(IdentityResult, string?)> CreateUserAsync(CoffeeShopUser user, string password)
        {
            IdentityResult result;

            var checkResult = await CheckIfUserExistsAsync(user);
            if (checkResult != null)
            {
                return (checkResult, null);
            }

            try
            {
                result = await userManager.CreateAsync(user, password);
            }
            catch
            {
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "UnknownError",
                    Description = "An unexpected error occurred while creating the user."
                }), null);
            }

            if (!result.Succeeded)
            {
                return (result, null);
            }

            return (result, user.Id);
        }

        private async Task<IdentityResult?> CheckIfUserExistsAsync(CoffeeShopUser user)
        {
            var errorMessages = new List<string>();
            CoffeeShopUser? existingUser;

            if (!string.IsNullOrEmpty(user.UserName))
            {
                existingUser = await userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName!.ToLower().Equals(user.UserName.ToLower()));

                if (existingUser != null)
                {
                    errorMessages.Add("Username is already taken.");
                }
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser = await userManager.Users
                    .FirstOrDefaultAsync(u => u.Email!.ToLower().Equals(user.Email.ToLower()));

                if (existingUser != null)
                {
                    errorMessages.Add("Email is already registered.");
                }
            }

            existingUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);

            if (existingUser != null)
            {
                errorMessages.Add("Phone number is already registered.");
            }

            if (errorMessages.Any())
            {
                var combinedErrorMessage = string.Join(" ", errorMessages);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateFields",
                    Description = combinedErrorMessage
                });
            }

            return null;
        }

        public async Task<IdentityResult> AddUserToRoleAsync(string userId, Roles role)
        {
            var roleName = role.ToString().ToUpper();

            var (result, existingUser) = await GetUserByIdAsync(userId);

            if (!result.Succeeded || existingUser == null)
                return result;

            if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }

            return await userManager.AddToRoleAsync(existingUser, roleName);
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

            if (!result.Succeeded || existingUser == null)
                return result;

            return await userManager.DeleteAsync(existingUser);
        }
    }
}
