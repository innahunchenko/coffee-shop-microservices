using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class EmailAddress : ValueOf<string, EmailAddress>
    {
        private static readonly Regex EmailAddressRegex =
            new("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!EmailAddressRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid email address";
                throw new ValidationException(message,
                [
                new ValidationFailure(nameof(EmailAddress), message)
            ]);
            }
        }
    }
}
