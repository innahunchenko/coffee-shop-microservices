using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.PaymentObjects
{
    public class CardNumber : ValueOf<string, CardNumber>
    {
        protected override void Validate()
        {
            var regex = new Regex(@"^\d{13,19}$");
            if (!regex.IsMatch(Value))
            {
                var message = "Invalid card number format";
                throw new ValidationException(message, [new ValidationFailure(nameof(CardNumber), message)]);
            }
        }
    }
}
