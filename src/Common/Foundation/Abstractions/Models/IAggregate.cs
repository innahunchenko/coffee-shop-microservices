namespace Foundation.Abstractions.Models
{
    public interface IAggregate
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        void ClearDomainEvents();
    }
}
