using Auth.API.Domain.Dtos;
using Auth.API.Domain.Models;
using Auth.API.Mapping;
using Auth.API.Repositories;
using Foundation.Abstractions.Services;
using LanguageExt.Pipes;
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
        public TokenUrlEncoderService TokenEncoder { get; set; }
        public LinkGenerator LinkGenerator { get; set; }
        public IHttpContextAccessor ContextAccessor { get; set; }
        private readonly string tokenCookieKey = "jwt-token";

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenGenerator,
            ICookieService cookieService,
            IOutboxRepository outboxRepository,
            IUnitOfWork unitOfWork,
            TokenUrlEncoderService encoder,
            LinkGenerator generator,
            IHttpContextAccessor contextAccessor)
        {
            this.cookieService = cookieService;
            this.userRepository = userRepository;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.unitOfWork = unitOfWork;
            this.outboxRepository = outboxRepository;
            TokenEncoder = encoder;
            LinkGenerator = generator;
            ContextAccessor = contextAccessor;
        }

        public async Task<(IdentityResult, CoffeeShopUser?)> GetUserByIdAsync(string userId)
        {
            var result = await userRepository.GetUserByIdAsync(userId);
            return result;
        }

        public async Task<(IdentityResult, CoffeeShopUser?)> GetUserByEmailAsync(string email)
        {
            var result = await userRepository.GetUserByEmailAsync(email);
            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(CoffeeShopUser user, string token, string newPassword)
        {
            var result = await userRepository.ResetPasswordAsync(user, token, newPassword);
            return result;
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

        public async Task<string> GetEmailConfirmationUrlAsync(CoffeeShopUser user, string route)
        {
            string token = await userRepository.GenerateEmailConfirmationTokenAsync(user);
            return GetUrl(user.Email!, token, route);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(CoffeeShopUser user)
        {
            string token = await userRepository.GeneratePasswordResetTokenAsync(user);
            return token;
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
