using Ordering.API.Domain.ValueObjects.OrderItemObjects;
namespace Ordering.API.Domain.Dtos
{
    public class OrderItemDto
    {
        public string OrderId { get; private set; } = default!;
        public string ProductId { get; private set; } = default!;
        public string ProductName { get; private set; } = default!;
        public int Quantity { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
    }
}
