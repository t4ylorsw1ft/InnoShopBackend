using InnoShop.ProductService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        Guid ProductId,
        string Name,
        string Description,
        double Price,
        bool IsAvailable,
        string? ImagePath,
        Guid UserId
    ) : IRequest<Product>;

}
