using InnoShop.ProductService.Application.UseCases.Products.DTOs;
using InnoShop.ProductService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(
        string? Name,
        int? MinPrice,
        int? MaxPrice,
        bool? IsAvailable,
        Guid? UserId
    ) : IRequest<List<ProductLookupDto>>;

}
