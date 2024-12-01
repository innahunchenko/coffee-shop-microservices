namespace Ordering.API.Domain.Dtos
{
    public class AddressDto
    {
        public string AddressLine { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
    }
}
