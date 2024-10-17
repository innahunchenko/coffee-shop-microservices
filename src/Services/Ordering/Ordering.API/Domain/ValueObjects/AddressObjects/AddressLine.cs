using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class AddressLine : ValueOf<string, AddressLine>
    {
        private static readonly Regex AddressLineRegex =
            new("^[a-zA-Z0-9\\s,.'-/]{1,100}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!AddressLineRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid address line";
                throw new ValidationException(message,
                [
                new ValidationFailure(nameof(AddressLine), message)
                ]);
            }
        }
    }
}
