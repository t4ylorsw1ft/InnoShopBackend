using InnoShop.UserService.Application.UseCases.DTOs;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Commands.Authentication.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<JwtPairDto>;
}
