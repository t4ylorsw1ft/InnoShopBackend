using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Users.Commands.Deactivate
{
    public record DeactivateCommand(Guid UserId) : IRequest;
}
