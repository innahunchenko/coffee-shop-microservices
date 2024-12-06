using Auth.API.Domain.Models;
using System.Net.Mail;

namespace Auth.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IJwtTokenService jwtTokenService;
        private readonly SmtpClient smtpClient;

        public EmailService(IJwtTokenService jwtTokenService, SmtpClient smtpClient) 
        {
            this.jwtTokenService = jwtTokenService;
            this.smtpClient = smtpClient;
        }

        public Task SendConfirmationEmailAsync(CoffeeShopUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
