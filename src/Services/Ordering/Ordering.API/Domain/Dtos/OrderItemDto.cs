namespace Ordering.API.Domain.Dtos
{
    public class OrderItemDto
    {
        public string OrderId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; } = default!;
        public decimal Price { get; set; } = default!;
    }
}
