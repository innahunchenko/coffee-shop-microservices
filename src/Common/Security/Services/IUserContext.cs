using Security.Models;

namespace Security.Services
{
    public interface IUserContext
    {
        string GetPhoneNumber();
        Roles GetUserRole();
        string? GetUserName();
        string GetUserEmail();
        string? GetUserId();
    }
}
