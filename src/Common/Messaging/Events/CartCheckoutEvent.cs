namespace Messaging.Events
{
    public class CartCheckoutEvent : IntegrationEvent
    {
        public List<ProductSelectionDto> ProductSelections { get; set; } = new();
        public string? UserId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string EmailAddress { get; set; } = default!;
        public string AddressLine { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        public string CardName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string Expiration { get; set; } = default!;
        public string CVV { get; set; } = default!;
    }

    public class ProductSelectionDto
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; } = default!;
    }
}
