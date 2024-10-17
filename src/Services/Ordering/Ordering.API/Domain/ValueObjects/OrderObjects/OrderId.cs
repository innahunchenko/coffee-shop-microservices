using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderObjects
{
    public class OrderId : ValueOf<Guid, OrderId>
    {
        protected override void Validate()
        {
            if (Value == Guid.Empty)
            {
                throw new ArgumentException("Order Id cannot be empty", nameof(OrderId));
            }
        }
    }
}
