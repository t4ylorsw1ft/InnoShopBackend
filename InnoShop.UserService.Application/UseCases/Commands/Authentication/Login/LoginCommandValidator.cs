using FluentValidation;

namespace InnoShop.UserService.Application.UseCases.Commands.Authentication.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(l => l.Email)
                    .NotEmpty().WithMessage("Email is required")
                    .EmailAddress().WithMessage("Invalid email format");

            RuleFor(l => l.Password)
                    .NotEmpty().WithMessage("Password is required")
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
        }
    }
}
