using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Users.Commands.SendVerificationEmail
{
    public record SendVerificationEmailCommand(Guid UserId) : IRequest;
}
