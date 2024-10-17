using Foundation.Abstractions.Models;
using Ordering.API.Domain.Events;
using Ordering.API.Domain.ValueObjects.AddressObjects;
using Ordering.API.Domain.ValueObjects.OrderObjects;
using Ordering.API.Domain.ValueObjects.PaymentObjects;

namespace Ordering.API.Domain.Models
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> orderItems = new();
        public IReadOnlyList<OrderItem> OrderItems => orderItems.AsReadOnly();
        public OrderName OrderName { get; private set; } = default!;
        public Address ShippingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Draft;
        public decimal TotalPrice
        {
            get => OrderItems.Sum(x => x.Price * x.Quantity);
            private set { }
        }

        public static Order Create(OrderName orderName, Address shippingAddress, Payment payment)
        {
            var order = new Order()
            {
                Id = OrderId.From(Guid.NewGuid()),
                OrderName = orderName,
                ShippingAddress = shippingAddress,
                Payment = payment,
                Status = OrderStatus.Pending
            };

            order.AddDomainEvent(new OrderCreatedEvent(order));
            return order;
        }
    }
}
