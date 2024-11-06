using Auth.API.Auth;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Auth.API.Validation
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.UserDto.FirstName)
                .NotEmpty().WithMessage("Required")
                .Matches(FirstLastNameRegex())
                .WithMessage("only letters and may include spaces, apostrophes, or hyphens.");

            RuleFor(x => x.UserDto.LastName)
                .NotEmpty().WithMessage("Required")
                .Matches(FirstLastNameRegex())
                .WithMessage("only letters and may include spaces, apostrophes, or hyphens.");

            RuleFor(x => x.UserDto.PhoneNumber)
                .NotEmpty().WithMessage("Required")
                .Matches(PhoneNumberRegex())
                .WithMessage("must be in a valid format, e.g., +1234567890.");

            RuleFor(x => x.UserDto.Email)
                .NotEmpty().WithMessage("Required")
                .EmailAddress()
                .WithMessage("must be in a valid format, e.g., example@domain.com.");

            RuleFor(x => x.UserDto.UserName)
                .NotEmpty().WithMessage("Required")
                .Length(3, 20).WithMessage("must be between 3 and 20 characters")
                .Matches(@"^[a-zA-Z0-9_.]+$").WithMessage("only letters, numbers, underscores, and dots.");

            RuleFor(x => x.UserDto.Password)
                .NotEmpty().WithMessage("Required")
                .MinimumLength(8).WithMessage("at least 8 characters long")
                .Matches(@"[A-Z]").WithMessage("at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("at least one lowercase letter")
                .Matches(@"\d").WithMessage("at least one number")
                .Matches(@"[@$!%*?&]").WithMessage("at least one special character.");

            RuleFor(x => x.UserDto.DateOfBirth)
                .Must(BeAValidDate).WithMessage("Must be a valid date")
                .Must(BePastDate).WithMessage("Date of Birth cannot be in the future.")
                .When(x => !string.IsNullOrEmpty(x.UserDto.DateOfBirth));
        }

        private bool BeAValidDate(string? dateOfBirth)
        {
            return DateTime.TryParse(dateOfBirth, out _);
        }

        private bool BePastDate(string? dateOfBirth)
        {
            return DateTime.TryParse(dateOfBirth, out var date) && date <= DateTime.Today;
        }

        private static Regex FirstLastNameRegex() =>
             new Regex("^[A-Za-z]+(?:[ '-][A-Za-z]+)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex PhoneNumberRegex() =>
             new Regex("^\\+?[1-9]\\d{1,14}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    }
}
