using Foundation.Abstractions.Models;
using Ordering.API.Domain.Dtos;
using Ordering.API.Domain.Events;
using Ordering.API.Domain.ValueObjects.AddressObjects;
using Ordering.API.Domain.ValueObjects.OrderItemObjects;
using Ordering.API.Domain.ValueObjects.OrderObjects;
using Ordering.API.Domain.ValueObjects.PaymentObjects;

namespace Ordering.API.Domain.Models
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> orderItems = [];
        public IReadOnlyList<OrderItem> OrderItems => orderItems.AsReadOnly();
        public OrderName OrderName { get => OrderName.From($"Order_{Id}"); set { } }
        public Address ShippingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Draft;
        public decimal TotalPrice
        {
            get => OrderItems.Sum(x => x.Price * x.Quantity);
            private set { }
        }

        public static Order Create(Address shippingAddress, Payment payment, OrderStatus orderStatus)
        {
            var order = new Order()
            {
                Id = OrderId.From(Guid.NewGuid()),
                ShippingAddress = shippingAddress,
                Payment = payment,
                Status = orderStatus
            };

            order.AddDomainEvent(new OrderCreatedEvent(order));
            return order;
        }

        public void AddItem(ProductId productId, ProductName productName, int quantity, decimal price) 
        {
            var orderItem = new OrderItem(Id, productId, productName, quantity, price);
            orderItems.Add(orderItem);
        }
    }
}
