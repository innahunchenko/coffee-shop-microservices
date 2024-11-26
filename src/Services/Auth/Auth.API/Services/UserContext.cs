using Auth.API.Domain.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.API.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Roles GetUserRole()
        {
            var jwtToken = GetJwtToken();
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            return string.IsNullOrEmpty(role) ? Roles.USER : Enum.Parse<Roles>(role, true);
        }

        public string? GetUserName()
        {
            var jwtToken = GetJwtToken();
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
        }

        private JwtSecurityToken GetJwtToken()
        {
            var token = httpContextAccessor?.HttpContext?.Request
                .Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
    }
}
