using Auth.API.Domain.Models;

namespace Auth.API.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(CoffeeShopUser user, CancellationToken cancellationToken);
    }
}
