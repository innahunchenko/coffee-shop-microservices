using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.PaymentObjects
{
    public class CVV : ValueOf<string, CVV>
    {
        protected override void Validate()
        {
            var regex = new Regex(@"^\d{3,4}$");
            if (!regex.IsMatch(Value))
            {
                var message = "Invalid CVV format";
                throw new ValidationException(message, [new ValidationFailure(nameof(CVV), message)]);
            }
        }
    }
}
