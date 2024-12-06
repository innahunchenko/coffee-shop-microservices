using Auth.API.Domain.Models;
using Auth.API.Services;
using MediatR;

namespace Auth.API.Auth
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
    {
       // private readonly IEmailService emailService;
        private readonly IAuthService authService;

        public UserRegisteredEventHandler(/*IEmailService emailService, */IAuthService authService)
        {
           // this.emailService = emailService;
            this.authService = authService;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var (result, user) = await authService.GetUserByIdAsync(notification.UserId);

            if (user is null)
            {
                return;
            }

           // await emailService.SendConfirmationEmailAsync(user, cancellationToken);
        }
    }
}
