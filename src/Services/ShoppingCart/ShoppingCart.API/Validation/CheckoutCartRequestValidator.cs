using FluentValidation;
using ShoppingCart.API.ShoppingCart;
using System.Text.RegularExpressions;

namespace ShoppingCart.API.Validation
{
    public class CheckoutCartRequestValidator : AbstractValidator<CheckoutCartRequest>
    {
        public CheckoutCartRequestValidator()
        {
            RuleFor(x => x.CartCheckoutDto.PhoneNumber)
                .Matches(PhoneNumberRegex())
                .WithMessage("Must be in a valid format, e.g., +1234567890.");

            RuleFor(x => x.CartCheckoutDto.EmailAddress)
                .Matches(EmailRegex())
                .WithMessage("Must be in a valid format, e.g., example@domain.com.");

            RuleFor(x => x.CartCheckoutDto.AddressLine)
                .Matches(AddressLineRegex())
                .WithMessage("Only letters, numbers, spaces, and special characters like ,.'#-.");

            RuleFor(x => x.CartCheckoutDto.Country)
                .Matches(CountryRegex())
                .WithMessage("Only letters and may include spaces or hyphens.");

            RuleFor(x => x.CartCheckoutDto.State)
                .Matches(StateRegex())
                .WithMessage("Only letters and may include spaces or hyphens.");

            RuleFor(x => x.CartCheckoutDto.ZipCode)
                .Matches(ZipCodeRegex())
                .WithMessage("Must be in a valid format (e.g., 12345 or A1B 2C3).");

            RuleFor(x => x.CartCheckoutDto.CardName)
                .Matches(CardNameRegex())
                .WithMessage("Only letters and spaces.");

            RuleFor(x => x.CartCheckoutDto.CardNumber)
                .Matches(CardNumberRegex())
                .WithMessage("Must be a valid credit/debit card number (e.g., 1234 5678 9012 3456).");

            RuleFor(x => x.CartCheckoutDto.Expiration)
                .Matches(ExpirationRegex())
                .WithMessage("Must be in the format MM/YY.");

            RuleFor(x => x.CartCheckoutDto.CVV)
                .Matches(CvvRegex())
                .WithMessage("Must be a 3 or 4 digit number.");
        }

        private static Regex PhoneNumberRegex() =>
             new Regex("^\\+?[1-9]\\d{1,14}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex AddressLineRegex() =>
            new Regex("^[a-zA-Z0-9\\s,.'#-]{1,100}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CountryRegex() =>
            new Regex("^[a-zA-ZÀ-ÿ\\s'-]{2,50}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex StateRegex() =>
            new Regex("^[a-zA-Z\\s-]{2,50}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex ZipCodeRegex() =>
            new Regex("^([A-Za-z0-9\\s-]{2,10}|(\\d{5}(-\\d{4})?)|([A-Za-z]\\d[A-Za-z]\\s\\d[A-Za-z]\\d)|([A-Za-z]{1,2}\\d[A-Za-z]?\\s?\\d[A-Za-z]{2})|\\d{4})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CardNameRegex() =>
            new Regex("^[a-zA-Z\\s]{2,50}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CardNumberRegex() =>
            new Regex("^(\\d{4}[- ]?){3}\\d{4}$|^\\d{13,19}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex ExpirationRegex() =>
            new Regex("^(0[1-9]|1[0-2])\\/([0-9]{2})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex CvvRegex() =>
            new Regex("^\\d{3,4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex EmailRegex() =>
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
