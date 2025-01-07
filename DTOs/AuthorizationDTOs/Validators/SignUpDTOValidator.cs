using FluentValidation;

namespace DTOs.AuthorizationDTOs.Validators
{
    public class SignUpDTOValidator : AbstractValidator<SignUpDTO>
    {
        public SignUpDTOValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(3, 50)
                .Matches(@"^[a-zA-Z0-9]+$")
                .WithMessage("Username must be 3-50 characters and can only contain letters and numbers.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of Birth is required.")
                .LessThan(DateTime.Today)
                .WithMessage("Date of Birth must be a date in the past.");
        }
    }
}
