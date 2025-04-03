using InnoShop.UserService.Application.UseCases.Users.DTOs;
using MediatR;

namespace Library.Application.UseCases.Users.Commands.Register
{
    public record RegisterCommand(string Username, string Email, string Password) : IRequest<JwtPairDto>; 
}
