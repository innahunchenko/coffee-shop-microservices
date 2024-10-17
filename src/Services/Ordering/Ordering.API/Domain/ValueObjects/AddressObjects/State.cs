using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.AddressObjects
{
    public class State : ValueOf<string, State>
    {
        private static readonly Regex StateRegex =
            new("^[a-zA-Z\\s-]{2,50}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!StateRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid state";
                throw new ValidationException(message,
                [
                new ValidationFailure(nameof(State), message)
                ]);
            }
        }

    }
}
