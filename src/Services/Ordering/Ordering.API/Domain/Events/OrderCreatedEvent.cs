using Foundation.Abstractions.Models;
using Ordering.API.Domain.Models;

namespace Ordering.API.Domain.Events
{
    public record OrderCreatedEvent(Guid Id, Order Order) : IDomainEvent;
}
