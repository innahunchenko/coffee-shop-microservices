using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderObjects
{
    public class OrderName : ValueOf<string, OrderName>
    {
        private static readonly Regex OrderNameRegex =
        new("^[a-zA-Z\\d](?:(?![-_]{2})[a-zA-Z\\d_-]{0,48}[a-zA-Z\\d])?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override void Validate()
        {
            if (!OrderNameRegex.IsMatch(Value))
            {
                var message = $"{Value} is not a valid order name";
                throw new ValidationException(message,
                [new ValidationFailure(nameof(OrderName), message)]);
            }
        }
    }
}
