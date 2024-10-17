namespace Foundation.Abstractions.Models
{
    public abstract class Aggregate<T> : Entity<T>, IAggregate
    {
        private readonly List<IDomainEvent> domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }
    }
}
