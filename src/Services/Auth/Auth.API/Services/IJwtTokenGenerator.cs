using Auth.API.Domain.Models;

namespace Auth.API.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(CoffeeShopUser user, IEnumerable<string> roles);
    }
}
