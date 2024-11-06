using Newtonsoft.Json;

namespace Auth.API.Domain.Dtos
{
    public class CoffeeShopUserDto
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }
        [JsonProperty("lastName")]
        public string? LastName { get; set; }
        [JsonProperty("email")]
        public string? Email { get; set; }
        [JsonProperty("userName")]
        public string? UserName { get; set; }
        [JsonProperty("phoneNumber")]
        public string? PhoneNumber { get; set; }
        [JsonProperty("password")]
        public string? Password { get; set; }
        [JsonProperty("dateOfBirth")]
        public string? DateOfBirth { get; set; }
    }
}
