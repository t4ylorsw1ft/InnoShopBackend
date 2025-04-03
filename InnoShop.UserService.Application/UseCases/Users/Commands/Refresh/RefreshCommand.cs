using InnoShop.UserService.Application.UseCases.Users.DTOs;
using MediatR;

namespace InnoShop.UserService.UseCases.Users.Commands.Refresh
{
    public record RefreshCommand(string RefreshToken) : IRequest<JwtPairDto>;
}
