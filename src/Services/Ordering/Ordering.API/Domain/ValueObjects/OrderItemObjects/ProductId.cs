using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderItemObjects
{
    public class ProductId : ValueOf<Guid, ProductId>
    {
        protected override void Validate()
        {
            if (Value == Guid.Empty)
            {
                throw new ArgumentException("Product Id cannot be empty", nameof(ProductId));
            }
        }
    }
}
