using Ordering.API.Domain.Models;
using Ordering.API.Domain.ValueObjects.OrderObjects;

namespace Ordering.API.Domain.Dtos
{
    public class OrderDto
    {
        public string? OrderName { get; set; }
        public string OrderStatus { get; set; } = "Created";
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public PaymentDto Payment { get; set; } = default!;
        public AddressDto ShippingAddress { get; set; } = default!;
        public PhoneNumber PhoneNumber { get; set; } = default!;
        public Email Email { get; set; } = default!;
        public decimal TotalPrice { get; set; } = default!;
        public string? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
