using InnoShop.UserService.Application.UseCases.DTOs;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Commands.Authentication.Refresh
{
    public record RefreshCommand(string RefreshToken) : IRequest<JwtPairDto>;
}
