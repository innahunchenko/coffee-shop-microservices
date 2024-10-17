using Foundation.Abstractions.Models;
using Ordering.API.Domain.ValueObjects.OrderItemObjects;
using Ordering.API.Domain.ValueObjects.OrderObjects;

namespace Ordering.API.Domain.Models
{
    public class OrderItem : Entity<OrderItemId>
    {
        public OrderId OrderId { get; private set; } = default!;
        public ProductId ProductId { get; private set; } = default!;
        public ProductName ProductName { get; private set; } = default!;
        public int Quantity { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;

        public OrderItem(OrderId orderId, ProductId productId, ProductName productName, int quantity, decimal price)
        {
            Id = OrderItemId.From(Guid.NewGuid());
            ProductId = productId;
            OrderId = orderId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }
    }
}
