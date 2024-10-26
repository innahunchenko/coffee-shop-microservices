using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderObjects
{
    public class PhoneNumber : ValueOf<string, PhoneNumber>
    {
        private static readonly Regex PhoneNumberRegex =
            new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

        protected override void Validate()
        {
            if (!PhoneNumberRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid international phone number";
                throw new ValidationException(message,
                [new ValidationFailure(nameof(PhoneNumber), message)]);
            }
        }
    }

}
