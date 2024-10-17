using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;
using ValueOf;

namespace Ordering.API.Domain.ValueObjects.OrderItemObjects
{
    public class ProductName : ValueOf<string, ProductName>
    {
        protected override void Validate()
        {
            var regex = new Regex(@"^[A-Za-z0-9][A-Za-z0-9\s-_]{1,48}[A-Za-z0-9]$");
            if (!regex.IsMatch(Value))
            {
                var message = "Invalid product name";
                throw new ValidationException(message, [new ValidationFailure(nameof(ProductName), message)]);
            }
        }
    }
}
