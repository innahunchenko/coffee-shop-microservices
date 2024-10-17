namespace Ordering.API.Domain.Dtos
{
    public class PaymentDto
    {
        public string? CardName { get; private set; }
        public string CardNumber { get; private set; } = default!;
        public string Expiration { get; private set; } = default!;
        public string CVV { get; private set; } = default!;
    }
}
