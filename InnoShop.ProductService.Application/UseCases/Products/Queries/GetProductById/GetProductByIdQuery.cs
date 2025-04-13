using InnoShop.ProductService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid ProductId) : IRequest<Product>;
}
