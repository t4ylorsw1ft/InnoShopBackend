using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Queries.IsUserActive
{
    public record IsUserActiveCommand(Guid UserId) : IRequest<bool>;
}
