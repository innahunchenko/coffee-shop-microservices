using Foundation.Abstractions.Models;

namespace Auth.API.Domain.Models
{
    public sealed record UserRegisteredEvent(Guid Id, string UserId) : BaseEvent(Id);
}
