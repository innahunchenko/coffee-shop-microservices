using Ordering.API.Domain.Models;

namespace Ordering.API.Domain.Dtos
{
    public class OrderDto
    {
        public string? OrderName { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Draft;
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public PaymentDto Payment { get; set; } = default!;
        public AddressDto ShippingAddress { get; set; } = default!;
        public decimal TotalPrice { get; set; } = default!;
    }
}
