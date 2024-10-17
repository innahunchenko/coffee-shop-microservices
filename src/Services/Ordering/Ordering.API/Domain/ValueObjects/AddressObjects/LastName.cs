using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class LastName : ValueOf<string, LastName>
    {
        private static readonly Regex LastNameRegex =
            new("^[a-z\\d](?:[a-z\\d]|-(?=[a-z\\d])){0,48}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!LastNameRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid last name";
                throw new ValidationException(message,
                [
                new ValidationFailure(nameof(LastName), message)
            ]);
            }
        }
    }
}
