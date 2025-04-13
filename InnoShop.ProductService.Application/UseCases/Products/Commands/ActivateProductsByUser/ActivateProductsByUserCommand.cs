using MediatR;


namespace InnoShop.ProductService.Application.UseCases.Products.Commands.ActivateProductsByUser
{
    public record ActivateProductsByUserCommand(Guid UserId) : IRequest;
}
