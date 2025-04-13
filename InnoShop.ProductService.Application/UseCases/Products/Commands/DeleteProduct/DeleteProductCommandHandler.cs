using FluentValidation;
using InnoShop.ProductService.Application.Common.Exceptions;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
                throw new NotFoundException(typeof(Product), request.ProductId);

            if (product.UserId != request.UserId)
                throw new AccessException("You can delete only your own products");

            await _productRepository.DeleteAsync(product, cancellationToken);
        }
    }

}
