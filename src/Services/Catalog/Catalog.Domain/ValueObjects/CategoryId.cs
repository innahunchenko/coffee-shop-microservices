namespace Catalog.Domain.ValueObjects
{
    public record CategoryId(Guid Value)
    {
        public static CategoryId New() => new CategoryId(Guid.NewGuid());
        public static CategoryId Empty() => new CategoryId(Guid.Empty);
        public static CategoryId Of(Guid value) => new CategoryId(value);
    }
}
