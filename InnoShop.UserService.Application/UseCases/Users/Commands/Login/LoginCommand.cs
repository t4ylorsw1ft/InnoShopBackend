using InnoShop.UserService.Application.UseCases.Users.DTOs;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Users.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<JwtPairDto>;
}
