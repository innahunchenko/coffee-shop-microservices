using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;
using Auth.API.Mapping;
using Auth.API.Repositories;
using Foundation.Abstractions.Services;
using Microsoft.AspNetCore.Identity;
using Security.Models;

namespace Auth.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtTokenService jwtTokenGenerator;
        private readonly ICookieService cookieService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IOutboxRepository outboxRepository;
        private readonly string tokenCookieKey = "jwt-token";

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenGenerator,
            ICookieService cookieService,
            IOutboxRepository outboxRepository,
            IUnitOfWork unitOfWork)
        {
            this.cookieService = cookieService;
            this.userRepository = userRepository;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.unitOfWork = unitOfWork;
            this.outboxRepository = outboxRepository;
        }

        public async Task<(IdentityResult, string?)> RegisterUserAsync(CoffeeShopUserDto dto, Roles role)
        {
            var checkResult = await CheckUserDuplicatesAsync(dto);
            if (!checkResult.Succeeded)
            {
                return (checkResult, null);
            }

            var user = dto.ToUser();

            await unitOfWork.BeginTransactionAsync();

            try
            {
                var (result, userId) = await userRepository.CreateUserAsync(user, dto.Password!, role);
                if (!result.Succeeded || string.IsNullOrEmpty(userId))
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return (result, null);
                }

                await outboxRepository.CreateAsync(new UserRegisteredEvent(Guid.NewGuid(), userId));

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                return (result, userId);
            }
            catch (Exception)
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
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
            cookieService.SetData(tokenCookieKey, token);
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
