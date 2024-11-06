namespace Foundation.Abstractions.Models
{
    public class Payment
    {
        public string? CardName { get; private set; } = default!;
        public string CardNumber { get; private set; } = default!;
        public string Expiration { get; private set; } = default!;
        public string CVV { get; private set; } = default!;

        public static Payment From(
            string? cardName, string cardNumber, string expiration, string cvv)
        {
            return new Payment
            {
                CardName = cardName,
                CardNumber = cardNumber,
                Expiration = expiration,
                CVV = cvv
            };
        }
    }
}
