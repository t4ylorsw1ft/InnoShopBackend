using FluentValidation;
using FluentValidation.Results;
using InnoShop.ProductService.Application.Common.Exceptions;
using InnoShop.ProductService.Application.UseCases.Products.Commands.UpdateProduct;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop_ProductService.Tests.Unit.Commands
{
    public class UpdateProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IValidator<UpdateProductCommand>> _validatorMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _validatorMock = new Mock<IValidator<UpdateProductCommand>>();
            _handler = new UpdateProductCommandHandler(_productRepositoryMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldUpdateProduct()
        {
            // arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var command = new UpdateProductCommand(
                productId,
                "Apple",
                "Desc",
                9.99,
                true,
                null,
                userId
            );

            var existingProduct = new Product
            {
                Id = productId,
                UserId = userId,
                Name = "OldName",
                Description = "OldDesc",
                Price = 5,
                IsAvaliable = false,
                ImagePath = null
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _productRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) => p);

            // act
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Price, result.Price);
            Assert.Equal(command.IsAvailable, result.IsAvaliable);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowNotFoundException()
        {
            // arrange
            var command = new UpdateProductCommand(
                Guid.NewGuid(),
                "Banana",
                "Desc",
                4.99,
                true,
                null,
                Guid.NewGuid()
            );

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(command.ProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // act and assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ProductNotOwnedByUser_ShouldThrowAccessException()
        {
            // arrange
            var productId = Guid.NewGuid();
            var command = new UpdateProductCommand(
                productId,
                "Banana",
                "Desc",
                4.99,
                true,
                null,
                Guid.NewGuid()
            );

            var existingProduct = new Product
            {
                Id = productId,
                UserId = Guid.NewGuid(),
                Name = "Banana",
                Description = "Desc",
                Price = 4.99
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            // act and assert
            await Assert.ThrowsAsync<AccessException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidCommand_ShouldThrowValidationException()
        {
            // arrange
            var command = new UpdateProductCommand(
                Guid.NewGuid(),
                "", // invalid name
                "Desc",
                0, // invalid price
                true,
                null,
                Guid.NewGuid()
            );

            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Product name is required"),
                new ValidationFailure("Price", "Price must be greater than 0")
            });

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Product name is required", ex.Message);
            Assert.Contains("Price must be greater than 0", ex.Message);
        }
    }
}