namespace Ordering.API.Domain.Dtos
{
    public class PaymentDto
    {
        public string? CardName { get; set; }
        public string CardNumber { get; set; } = default!;
        public string Expiration { get; set; } = default!;
        public string CVV { get; set; } = default!;
    }
}
