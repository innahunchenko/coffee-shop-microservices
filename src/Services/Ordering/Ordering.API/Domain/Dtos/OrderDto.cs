using Ordering.API.Domain.Models;

namespace Ordering.API.Domain.Dtos
{
    public class OrderDto
    {
        public string Id { get; set; } = default!;
        public string OrderName { get; set; } = default!;
        public OrderStatus OrderStatus { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public PaymentDto Payment { get; set; } = default!;
        public AddressDto Address { get; set; } = default!;

    }
}
