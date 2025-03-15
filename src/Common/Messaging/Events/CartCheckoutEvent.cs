using System.Text.Json.Serialization;

namespace Messaging.Events
{
    public class CartCheckoutEvent : IntegrationEvent
    {
        [JsonPropertyName("productSelections")]
        public List<ProductSelectionDto> ProductSelections { get; set; } = new List<ProductSelectionDto>();

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = default!;

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = default!;

        [JsonPropertyName("addressLine")]
        public string AddressLine { get; set; } = default!;

        [JsonPropertyName("country")]
        public string Country { get; set; } = default!;

        [JsonPropertyName("state")]
        public string State { get; set; } = default!;

        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; } = default!;

        [JsonPropertyName("cardName")]
        public string CardName { get; set; } = default!;

        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; } = default!;

        [JsonPropertyName("expiration")]
        public string Expiration { get; set; } = default!;

        [JsonPropertyName("cvv")]
        public string CVV { get; set; } = default!;
    }

    public class ProductSelectionDto
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = default!;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = default!;
    }
}
