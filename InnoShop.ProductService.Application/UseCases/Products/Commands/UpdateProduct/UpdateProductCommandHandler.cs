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

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<UpdateProductCommand> _validator;

        public UpdateProductCommandHandler(IProductRepository productRepository, IValidator<UpdateProductCommand> validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingProduct = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (existingProduct == null)
                throw new NotFoundException(typeof(Product), request.ProductId);

            if (existingProduct.UserId != request.UserId)
                throw new AccessException("You can update only your own products");

            existingProduct.Name = request.Name;
            existingProduct.Description = request.Description;
            existingProduct.Price = request.Price;
            existingProduct.IsAvaliable = request.IsAvailable;
            existingProduct.ImagePath = request.ImagePath;

            return await _productRepository.UpdateAsync(existingProduct, cancellationToken);
        }
    }

}
