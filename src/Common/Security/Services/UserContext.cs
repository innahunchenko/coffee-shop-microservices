using Microsoft.AspNetCore.Http;
using Security.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Security.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetPhoneNumber()
        {
            var jwtToken = GetJwtToken();
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == "phone_number")?.Value ?? string.Empty;
        }

        public Roles GetUserRole()
        {
            var jwtToken = GetJwtToken();
            var role = jwtToken?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            return string.IsNullOrEmpty(role) ? Roles.USER : Enum.Parse<Roles>(role, true);
        }

        public string? GetUserName()
        {
            var jwtToken = GetJwtToken();
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
        }

        public string? GetUserId()
        {
            var jwtToken = GetJwtToken();
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }

        public string GetUserEmail()
        {
            var jwtToken = GetJwtToken();
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;
        }

        private JwtSecurityToken? GetJwtToken()
        {
            var token = httpContextAccessor?.HttpContext?.Request
                .Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                return null;
            }

            token = token.Replace("Bearer ", string.Empty);

            try
            {
                var handler = new JwtSecurityTokenHandler();
                return handler.ReadJwtToken(token);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("The JWT token is invalid or improperly formatted.");
            }
        }
    }
}
