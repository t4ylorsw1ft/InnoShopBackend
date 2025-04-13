using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(Guid ProductId, Guid UserId) : IRequest;

}
