using FluentValidation;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.UseCases.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<CreateProductCommand> _validator;

        public CreateProductCommandHandler(IProductRepository productRepository, IValidator<CreateProductCommand> validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsAvaliable = true,
                UserId = request.UserId,
                CreationDateTime = DateTime.UtcNow,
                ImagePath = request.ImagePath,
                IsActive = true
            };

            return await _productRepository.CreateAsync(product, cancellationToken);
        }
    }

}
