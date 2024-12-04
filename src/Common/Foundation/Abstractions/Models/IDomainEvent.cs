using MediatR;

namespace Foundation.Abstractions.Models
{
    public interface IDomainEvent : INotification
    {
        public Guid Id { get; init; }
    }
}
