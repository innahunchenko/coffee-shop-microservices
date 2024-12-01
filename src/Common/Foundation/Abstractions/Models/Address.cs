namespace Foundation.Abstractions.Models
{
    public class Address
    {
        public string AddressLine { get; private set; } = default!;
        public string Country { get; private set; } = default!;
        public string State { get; private set; } = default!;
        public string ZipCode { get; private set; } = default!;

        public static Address From(string addressLine, string country, string state, string zipCode)
        {
            return new Address
            {
                AddressLine = addressLine,
                Country = country,
                State = state,
                ZipCode = zipCode
            };
        }
    }
}
