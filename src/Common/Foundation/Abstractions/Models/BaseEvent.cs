namespace Foundation.Abstractions.Models
{
    public abstract record BaseEvent(Guid Id) : IDomainEvent;
}
