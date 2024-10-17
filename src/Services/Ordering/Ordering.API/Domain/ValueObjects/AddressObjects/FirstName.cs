using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class FirstName : ValueOf<string, FirstName>
    {
        private static readonly Regex FirstNameRegex =
            new("^[a-z\\d](?:[a-z\\d]|-(?=[a-z\\d])){0,38}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!FirstNameRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid first name";
                throw new ValidationException(message,
                [
                new ValidationFailure(nameof(FirstName), message)
            ]);
            }
        }
    }
}
