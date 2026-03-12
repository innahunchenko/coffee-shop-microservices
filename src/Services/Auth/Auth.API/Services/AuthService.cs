using Auth.API.Data;
using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;
using Auth.API.Mapping;
using Auth.API.Repositories;
using Foundation.Abstractions.Services;
using LanguageExt.Pipes;
using Microsoft.AspNetCore.Identity;
using Security.Models;
using System.Transactions;

namespace Auth.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtTokenService jwtTokenGenerator;
        private readonly ICookieService cookieService;
        private readonly IOutboxRepository outboxRepository;
        private readonly AppDbContext appDbContext;

        public TokenUrlEncoderService TokenEncoder { get; set; }
        public LinkGenerator LinkGenerator { get; set; }
        public IHttpContextAccessor ContextAccessor { get; set; }

        private readonly string tokenCookieKey = "jwt-token";

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenGenerator,
            ICookieService cookieService,
            IOutboxRepository outboxRepository,
            AppDbContext appDbContext,
            TokenUrlEncoderService encoder,
            LinkGenerator generator,
            IHttpContextAccessor contextAccessor)
        {
            this.cookieService = cookieService;
            this.userRepository = userRepository;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.outboxRepository = outboxRepository;
            this.appDbContext = appDbContext;
            TokenEncoder = encoder;
            LinkGenerator = generator;
            ContextAccessor = contextAccessor;
        }

        public async Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId)
        {
            return await userRepository.GetUserByIdAsync(userId);
        }

        public async Task<(IdentityResult, CoffeeShopUser?)> GetUserByEmailAsync(string email)
        {
            return await userRepository.GetUserByEmailAsync(email);
        }

        public async Task<IdentityResult> ResetPasswordAsync(CoffeeShopUser user, string token, string newPassword)
        {
            return await userRepository.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<(IdentityResult, string?)> RegisterUserAsync(
            CoffeeShopUserDto dto,
            Roles role)
        {
            var checkResult = await CheckUserDuplicatesAsync(dto);
            if (!checkResult.Succeeded)
            {
                return (checkResult, null);
            }

            var user = dto.ToUser();

            // TransactionScope ensures atomicity between Identity and Outbox
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var (result, userId) = await userRepository.CreateUserAsync(user, dto.Password!, role);
            if (!result.Succeeded || string.IsNullOrEmpty(userId))
            {
                // TransactionScope will not be completed → transaction is rolled back
                return (result, null);
            }

            await outboxRepository.CreateAsync(new UserRegisteredEvent(Guid.NewGuid(), userId));

            // SaveChanges is called directly on AppDbContext only for Outbox
            await appDbContext.SaveChangesAsync();

            scope.Complete();

            return (result, userId);
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

            if (user == null || !await userRepository.CheckPasswordAsync(user, password))
            {
                return failedResult;
            }

            var roles = await userRepository.GetRolesAsync(user);
            var token = jwtTokenGenerator.GenerateToken(user, roles);
            cookieService.SetData(tokenCookieKey, token);

            return IdentityResult.Success;
        }

        public async Task<string> GetEmailConfirmationUrlAsync(CoffeeShopUser user, string route)
        {
            string token = await userRepository.GenerateEmailConfirmationTokenAsync(user);
            return GetUrl(user.Email!, token, route);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(CoffeeShopUser user)
        {
            return await userRepository.GeneratePasswordResetTokenAsync(user);
        }

        private string GetUrl(string emailAddress, string token, string route)
        {
            string safeToken = TokenEncoder.EncodeToken(token);
            var url = $"{ContextAccessor.HttpContext.Request.Scheme}://{ContextAccessor.HttpContext.Request.Host}/{route}?email={emailAddress}&token={safeToken}";
            return url;
        }

        private async Task<IdentityResult> CheckUserDuplicatesAsync(CoffeeShopUserDto userDto)
        {
            var errorMessages = new List<string>();

            var user = await userRepository.FindUserByConditionAsync(
                u => u.UserName != null && u.UserName.ToLower() == userDto.UserName.ToLower());
            if (user != null) errorMessages.Add("Username is already taken.");

            user = await userRepository.FindUserByConditionAsync(
                u => u.Email != null && u.Email.ToLower() == userDto.Email.ToLower());
            if (user != null) errorMessages.Add("Email is already registered.");

            user = await userRepository.FindUserByConditionAsync(
                u => u.PhoneNumber != null && u.PhoneNumber.ToLower() == userDto.PhoneNumber.ToLower());
            if (user != null) errorMessages.Add("Phone number is already registered.");

            if (errorMessages.Any())
            {
                return IdentityResult.Failed(
                    errorMessages.Select(msg => new IdentityError
                    {
                        Code = "DuplicateField",
                        Description = msg
                    }).ToArray());
            }

            return IdentityResult.Success;
        }
    }
}
