using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;
using Auth.API.Mapping;
using Auth.API.Repositories;
using Foundation.Abstractions.Services;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtTokenService jwtTokenGenerator;
        private readonly ICookieService cookieService;
        private readonly string cookieKey = "jwt-token";

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenGenerator,
            ICookieService cookieService)
        {
            this.cookieService = cookieService;
            this.userRepository = userRepository;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<(IdentityResult, string?)> RegisterUserAsync(CoffeeShopUserDto dto)
        {
            var checkResult = await CheckUserDuplicatesAsync(dto);
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

        public async Task<IdentityResult> LoginUserAsync(string userName, string password)
        {
            var failedResult = IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidCredentials",
                Description = "The username or password is incorrect."
            });

            var user = await userRepository.FindUserByConditionAsync(
                u => u.UserName != null && u.UserName.ToLower() == userName.ToLower());

            if (user == null)
            {
                return failedResult;
            }

            bool isValid = await userRepository.CheckPasswordAsync(user, password);

            if (!isValid)
            {
                return failedResult;
            }

            var roles = await userRepository.GetRolesAsync(user);
            var token = jwtTokenGenerator.GenerateToken(user, roles);
            cookieService.SetData(cookieKey, token);
            return IdentityResult.Success;
        }

        private async Task<IdentityResult> CheckUserDuplicatesAsync(CoffeeShopUserDto userDto)
        {
            var errorMessages = new List<string>();
            var user = await userRepository
                .FindUserByConditionAsync(u => u.UserName != null && u.UserName.ToLower() == userDto.UserName.ToLower());

            if (user != null)
            {
                errorMessages.Add("Username is already taken.");
            }

            user = await userRepository
                .FindUserByConditionAsync(u => u.Email != null && u.Email.ToLower() == userDto.Email.ToLower());

            if (user != null)
            {
                errorMessages.Add("Email is already registered.");
            }

            user = await userRepository
                .FindUserByConditionAsync(u => u.PhoneNumber != null && u.PhoneNumber.ToLower() == userDto.PhoneNumber.ToLower());

            if (user != null)
            {
                errorMessages.Add("Phone number is already registered.");
            }

            if (errorMessages.Any())
            {
                return IdentityResult.Failed(errorMessages.Select(msg => new IdentityError
                {
                    Code = "DuplicateField",
                    Description = msg
                }).ToArray());
            }

            return IdentityResult.Success;
        }
    }
}
