using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderItemObjects
{
    public class OrderItemId : ValueOf<Guid, OrderItemId>
    {
        protected override void Validate()
        {
            if (Value == Guid.Empty)
            {
                throw new ArgumentException("Order item Id cannot be empty", nameof(OrderItemId));
            }
        }
    }
}
