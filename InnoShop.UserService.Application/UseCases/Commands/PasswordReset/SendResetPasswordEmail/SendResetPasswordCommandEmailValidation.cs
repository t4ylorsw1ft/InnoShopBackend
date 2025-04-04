using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Commands.PasswordReset.SendResetPasswordEmail
{
    public class SendResetPasswordCommandEmailValidation : AbstractValidator<SendResetPasswordEmailCommand>
    {
        public SendResetPasswordCommandEmailValidation()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
