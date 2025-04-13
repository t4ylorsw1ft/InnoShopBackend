using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.DeactivateProductsByUser
{
    public record DeactivateProductsByUserCommand(Guid UserId) : IRequest;
}
