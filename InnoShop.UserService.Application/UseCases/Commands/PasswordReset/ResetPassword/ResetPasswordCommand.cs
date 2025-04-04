using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Commands.PasswordReset.ResetPassword
{
    public record ResetPasswordCommand(string Email, string ResetPasswordCode, string NewPassword, string NewPasswordConfirmation) : IRequest;
}
