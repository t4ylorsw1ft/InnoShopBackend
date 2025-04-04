using InnoShop.UserService.Application.UseCases.DTOs;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Commands.Authentication.Register
{
    public record RegisterCommand(string Username, string Email, string Password) : IRequest<JwtPairDto>;
}
