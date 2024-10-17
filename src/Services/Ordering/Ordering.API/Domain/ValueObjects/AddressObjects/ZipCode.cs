using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class ZipCode : ValueOf<string, ZipCode>
    {
        private static readonly Regex ZipCodeRegex =
            new("^[a-zA-Z0-9\\s-]{3,10}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!ZipCodeRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid zip code";
                throw new ValidationException(message,
                [
                new ValidationFailure(nameof(ZipCode), message)
            ]);
            }
        }
    }
}
