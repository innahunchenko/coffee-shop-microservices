using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.PaymentObjects
{
    public class CardName : ValueOf<string, CardName>
    {
        protected override void Validate()
        {
            var regex = new Regex("^[a-zA-Z\\s]{2,50}$");
            if (!regex.IsMatch(Value))
            {
                var message = $"Invalid card name format";
                throw new ValidationException(message, [new ValidationFailure(nameof(CardName), message)]);
            }
        }
    }
}