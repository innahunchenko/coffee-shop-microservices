using Foundation.Abstractions.Models;
using Ordering.API.Domain.Events;
using Ordering.API.Domain.ValueObjects.OrderItemObjects;
using Ordering.API.Domain.ValueObjects.OrderObjects;

namespace Ordering.API.Domain.Models
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> orderItems = [];
        public IReadOnlyList<OrderItem> OrderItems => orderItems.AsReadOnly();
        public OrderName OrderName { get => OrderName.From($"Order_{Id}"); private set { } }
        public PhoneNumber PhoneNumber { get; private set; } = default!;
        public Email Email { get; private set; } = default!;
        public Address ShippingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public OrderStatus? Status { get; private set; } = OrderStatus.Created;
        public string? UserId { get; private set; }
        public decimal TotalPrice
        {
            get => OrderItems.Sum(x => x.Price * x.Quantity);
            private set { }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Order other) return false;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static Order Create(Address shippingAddress, 
            Payment payment, 
            OrderStatus orderStatus,
            PhoneNumber phoneNumber, 
            Email email,
            string? userId = null)
        {
            var order = new Order()
            {
                Id = OrderId.From(Guid.NewGuid()),
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber,
                Payment = payment,
                Status = orderStatus,
                Email = email,
                UserId = userId
            };

            order.AddDomainEvent(new OrderCreatedEvent(Guid.NewGuid(), order));
            return order;
        }

        public void AddItem(ProductId productId, ProductName productName, int quantity, decimal price) 
        {
            var orderItem = new OrderItem(Id, productId, productName, quantity, price);
            orderItems.Add(orderItem);
        }
    }
}
