using FluentValidation;
using ShoppingCart.API.ShoppingCart;
using System.Text.RegularExpressions;

namespace ShoppingCart.API.Validation
{
    public class CheckoutCartRequestValidator : AbstractValidator<CheckoutCartRequest>
    {
        private readonly string validationMessage = "This field is not invalid.";

        public CheckoutCartRequestValidator() 
        {
            RuleFor(x => x.CartCheckoutDto.FirstName)
                .Matches(FirstLastNameRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.LastName)
                .Matches(FirstLastNameRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.EmailAddress)
                .EmailAddress()
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.AddressLine)
                .Matches(AddressLineRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.Country)
                .Matches(CountryRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.State)
                .Matches(StateRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.ZipCode)
                .Matches(ZipCodeRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.CardName)
                .Matches(CardNameRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.CardNumber)
                .Matches(CardNumberRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.Expiration)
                .Matches(ExpirationRegex())
                .WithMessage(validationMessage);

            RuleFor(x => x.CartCheckoutDto.CVV)
                .Matches(CvvRegex())
                .WithMessage(validationMessage);
        }

        private static Regex FirstLastNameRegex() =>
             new Regex("^[A-Za-z]+(?:[ '-][A-Za-z]+)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex AddressLineRegex() =>
            new Regex("^[a-zA-Z0-9\\s,.'#-]{1,100}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CountryRegex() =>
            new Regex("^[a-zA-ZÀ-ÿ\\s'-]{2,50}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex StateRegex() =>
            new Regex("^[a-zA-Z\\s-]{2,50}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex ZipCodeRegex() =>
            new Regex("^([A-Za-z0-9\\s-]{2,10}|(\\d{5}(-\\d{4})?)|([A-Za-z]\\d[A-Za-z]\\s\\d[A-Za-z]\\d)|([A-Za-z]{1,2}\\d[A-Za-z]?\\s?\\d[A-Za-z]{2})|\\d{4})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CardNameRegex()
            => new Regex("^[a-zA-Z\\s]{2,50}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CardNumberRegex() =>
            new Regex("^(\\d{4}[- ]?){3}\\d{4}$|^\\d{13,19}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex ExpirationRegex() =>
            new Regex("^(0[1-9]|1[0-2])\\/([0-9]{2})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CvvRegex() =>
            new Regex("^\\d{3,4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
