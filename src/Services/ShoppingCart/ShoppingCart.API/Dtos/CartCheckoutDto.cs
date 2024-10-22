using Newtonsoft.Json;

namespace ShoppingCart.API.Dtos
{
    public class CartCheckoutDto
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; } = default!;
        [JsonProperty("lastName")]
        public string LastName { get; set; } = default!;
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; } = default!;
        [JsonProperty("addressLine")]
        public string AddressLine { get; set; } = default!;
        [JsonProperty("country")]
        public string Country { get; set; } = default!;
        [JsonProperty("state")]
        public string State { get; set; } = default!;
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; } = default!;
        [JsonProperty("cardName")]
        public string CardName { get; set; } = default!;
        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; } = default!;
        [JsonProperty("expiration")]
        public string Expiration { get; set; } = default!;
        [JsonProperty("cvv")]
        public string CVV { get; set; } = default!;
    }
}
