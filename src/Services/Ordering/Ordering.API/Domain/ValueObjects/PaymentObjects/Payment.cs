namespace Ordering.API.Domain.ValueObjects.PaymentObjects
{
    public class Payment
    {
        public CardName? CardName { get; private set; } = default!;
        public CardNumber CardNumber { get; private set; } = default!;
        public Expiration Expiration { get; private set; } = default!;
        public CVV CVV { get; private set; } = default!;

        public static Payment From(
            CardName? cardName, CardNumber cardNumber, Expiration expiration, CVV cvv)
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
