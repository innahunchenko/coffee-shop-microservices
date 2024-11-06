using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;
using Auth.API.Mapping;
using Auth.API.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Services
{
    public class AuthService(IUserRepository userRepository) : IAuthService
    {
        public async Task<(IdentityResult, string?)> RegisterUserAsync(CoffeeShopUserDto dto)
        {
            var user = dto.ToUser();
            var (result, userId) = await userRepository.CreateUserAsync(user, dto.Password!);
            return (result, userId);
        }

        public async Task<IdentityResult> AddUserToUserRoleAsync(string userId)
        {
            var result = await userRepository.AddUserToRoleAsync(userId, Roles.User);
            return result;
        }
    }
}
