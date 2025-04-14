using InnoShop.ProductService.Domain.Entities;
using MediatR;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Name,
        string Description,
        double Price,
        string? ImagePath,
        Guid UserId
    ) : IRequest<Product>;

}
