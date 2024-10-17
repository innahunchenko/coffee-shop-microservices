using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class Country : ValueOf<string, Country>
    {
        private static readonly Regex CountryRegex =
            new("^[a-zA-Z\\s-]{2,50}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!CountryRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid country";
                throw new ValidationException(message,
                [
                new ValidationFailure(nameof(Country), message)
                ]);
            }
        }
    }
}
