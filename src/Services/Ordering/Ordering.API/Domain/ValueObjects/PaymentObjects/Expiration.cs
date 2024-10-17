using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.PaymentObjects
{
    public class Expiration : ValueOf<string, Expiration>
    {
        protected override void Validate()
        {
            var regex = new Regex(@"^(0[1-9]|1[0-2])\/([0-9]{2})$");
            if (!regex.IsMatch(Value))
            {
                var message = "Invalid expiration date format";
                throw new ValidationException(message, [new ValidationFailure(nameof(Expiration), message)]);
            }
        }
    }
}

