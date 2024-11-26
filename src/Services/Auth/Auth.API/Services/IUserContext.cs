using Auth.API.Domain.Models;

namespace Auth.API.Services
{
    public interface IUserContext
    {
        Roles GetUserRole();
        string? GetUserName();
    }
}
