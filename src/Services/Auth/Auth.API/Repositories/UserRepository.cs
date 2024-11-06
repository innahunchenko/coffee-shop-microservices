using Auth.API.Domain.Models;
using Foundation.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<CoffeeShopUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        //const int UniqueConstraintViolationErrorCode1 = 2601;
        //const int UniqueConstraintViolationErrorCode2 = 2627;

        public UserRepository(UserManager<CoffeeShopUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IEnumerable<CoffeeShopUser> GetAllUsersAsync()
        {
            return userManager.Users.ToList();
        }

        public async Task<CoffeeShopUser?> GetUserByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
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
            //catch (Exception ex) when (IsSqlUniqueConstraintViolation(ex).isDuplicate)
            //{
            //    var (isDuplicate, errorMessage) = IsSqlUniqueConstraintViolation(ex);

            //    string description = errorMessage.Contains(nameof(CoffeeShopUser.PhoneNumber)) 
            //        ? $"This {nameof(CoffeeShopUser.PhoneNumber)} field is already registered."
            //        : "A unique constraint violation occurred.";

            //    return (IdentityResult.Failed(new IdentityError
            //    {
            //        Code = "DuplicateField",
            //        Description = description
            //    }), null);
            //}
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

        //private static (bool isDuplicate, string errorMessage) IsSqlUniqueConstraintViolation(Exception ex)
        //{
        //    if (ex.InnerException is SqlException sqlEx &&
        //        (sqlEx.Number == UniqueConstraintViolationErrorCode1 || sqlEx.Number == UniqueConstraintViolationErrorCode2))
        //    {
        //        return (true, sqlEx.Message);
        //    }
        //    return (false, string.Empty);
        //}

        public async Task<IdentityResult> UpdateUserAsync(CoffeeShopUser user)
        {
            var existingUser = await userManager.FindByIdAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;
                existingUser.PhoneNumber = user.PhoneNumber;
                return await userManager.UpdateAsync(existingUser);
            }
            return IdentityResult.Failed();
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
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"There is no user with Id {userId}");
            return await userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> AddUserToRoleAsync(string userId, Roles role)
        {
            var roleName = role.ToString().ToUpper();
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException($"There is no user with Id {userId}");

            if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }

            return await userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(CoffeeShopUser user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(CoffeeShopUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
    }
}
