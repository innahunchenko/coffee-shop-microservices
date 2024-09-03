namespace Catalog.Domain.ValueObjects
{
    public record ProductId
    {
        private ProductId(Guid value) => Value = value;
        public Guid Value { get; }
        public static ProductId Of(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new Exception("ProductId cannot be empty.");
            }

            return new ProductId(value);
        }
    }
}
