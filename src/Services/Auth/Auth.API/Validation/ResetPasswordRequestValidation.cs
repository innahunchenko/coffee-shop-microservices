using Auth.API.Auth;
using FluentValidation;

namespace Auth.API.Validation
{
    public class ResetPasswordRequestValidation : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidation() 
        {
            RuleFor(x => x.ResetPasswordDto.Password)
                .NotEmpty().WithMessage("Required")
                .MinimumLength(8).WithMessage("at least 8 characters long")
                .Matches(@"[A-Z]").WithMessage("at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("at least one lowercase letter")
                .Matches(@"\d").WithMessage("at least one number")
                .Matches(@"[@$!%*?&]").WithMessage("at least one special character.");

            RuleFor(x => x.ResetPasswordDto.ConfirmPassword)
                .NotEmpty().WithMessage("Required")
                .MinimumLength(8).WithMessage("at least 8 characters long")
                .Matches(@"[A-Z]").WithMessage("at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("at least one lowercase letter")
                .Matches(@"\d").WithMessage("at least one number")
                .Matches(@"[@$!%*?&]").WithMessage("at least one special character.");

            RuleFor(x => x)
            .Must(x => x.ResetPasswordDto.Password == x.ResetPasswordDto.ConfirmPassword)
            .WithMessage("Passwords do not match.");
        }
    }
}
