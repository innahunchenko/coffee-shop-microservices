using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderObjects
{
    public class Email : ValueOf<string, Email>
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        protected override void Validate()
        {
            if (!EmailRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid email";
                throw new ValidationException(message,
                [new ValidationFailure(nameof(Email), message)]);
            }
        }
    }
}
