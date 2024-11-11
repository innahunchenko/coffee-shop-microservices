using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;
using Auth.API.Mapping;
using Auth.API.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Auth.API.Services
{
    public class AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, ITokenProvider tokenProvider) : IAuthService
    {
        public async Task<(IdentityResult, string?)> RegisterUserAsync(CoffeeShopUserDto dto)
        {
            var (checkResult, _) = await CheckIfUserExistsAsync(dto);
            if (!checkResult.Succeeded)
            {
                return (checkResult, null);
            }

            var user = dto.ToUser();
            var (result, userId) = await userRepository.CreateUserAsync(user, dto.Password!);
            return (result, userId);
        }

        public async Task<IdentityResult> AddUserToUserRoleAsync(string userId)
        {
            var result = await userRepository.AddUserToRoleAsync(userId, Roles.User);
            return result;
        }

        public async Task<IdentityResult> LoginUserAsync(CoffeeShopUserDto dto)
        {
            var (checkResult, user) = await CheckIfUserExistsAsync(dto);
            if (checkResult.Succeeded)
            {
                return checkResult;
            }

            //var user = dto.ToUser();

            bool isValid = await userRepository.CheckPasswordAsync(user, dto.Password!);

            if (!isValid)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidPassword",
                    Description = "The password provided is incorrect. Please try again."
                });
            }

            var roles = await userRepository.GetRolesAsync(user);
            var token = jwtTokenGenerator.GenerateToken(user, roles);
            tokenProvider.SetToken(token);
            return IdentityResult.Success;
        }

        private async Task<(IdentityResult, CoffeeShopUser?)> CheckIfUserExistsAsync(CoffeeShopUserDto userDto)
        {
            var errorMessages = new List<string>();
            CoffeeShopUser? existingUser = null;

            // The `??=` operator assigns `existingUser` only if it has not already been set.
            // This ensures that `existingUser` retains the first matching user found,
            // even if subsequent checks also find matches. All error messages, however,
            // will still be collected in `errorMessages` regardless of whether `existingUser`
            // has been assigned.

            existingUser ??= await CheckAndAddErrorAsync(userDto.UserName,
                                                         u => u.UserName != null && u.UserName.ToLower() == userDto.UserName.ToLower(),
                                                         "Username is already taken.",
                                                         errorMessages);

            existingUser ??= await CheckAndAddErrorAsync(userDto.Email,
                                                         u => u.Email != null && u.Email.ToLower() == userDto.Email.ToLower(),
                                                         "Email is already registered.",
                                                         errorMessages);

            existingUser ??= await CheckAndAddErrorAsync(userDto.PhoneNumber,
                                                         u => u.PhoneNumber != null && u.PhoneNumber.ToLower() == userDto.PhoneNumber.ToLower(),
                                                         "Phone number is already registered.",
                                                         errorMessages);

            if (errorMessages.Any() && existingUser != null)
            {
                var combinedErrorMessage = string.Join(" ", errorMessages);
                return (IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateFields",
                    Description = combinedErrorMessage
                }), existingUser);
            }

            return (IdentityResult.Success, existingUser);
        }

        private async Task<CoffeeShopUser?> CheckAndAddErrorAsync(
            string? fieldValue,
            Expression<Func<CoffeeShopUser, bool>> predicate,
            string errorMessage,
            List<string> errorMessages)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                var user = await userRepository.FindUserByConditionAsync(predicate);
                if (user != null)
                {
                    errorMessages.Add(errorMessage);
                    return user;
                }
            }
            return null;
        }
    }
}
