using FluentValidation;
using FluentValidation.Results;
using InnoShop.ProductService.Application.UseCases.Products.Commands.CreateProduct;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using Moq;

namespace InnoShop_ProductService.Tests.Unit.Commands
{
    public class CreateProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IValidator<CreateProductCommand>> _validatorMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _validatorMock = new Mock<IValidator<CreateProductCommand>>();
            _handler = new CreateProductCommandHandler(
                _productRepositoryMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCreateProduct()
        {
            // arrange
            var command = new CreateProductCommand(
                Name: "Apple",
                Description: "Desc",
                Price: 19.99,
                ImagePath: "apple.jpg",
                UserId: Guid.NewGuid());

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                UserId = command.UserId,
                ImagePath = command.ImagePath,
                IsAvaliable = true,
                IsActive = true,
                CreationDateTime = DateTime.UtcNow
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _productRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // act
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Price, result.Price);
            Assert.Equal(command.UserId, result.UserId);
            Assert.True(result.IsAvaliable);
            Assert.True(result.IsActive);

            _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ShouldThrowValidationException_EmptyName()
        {
            // arrange
            var command = new CreateProductCommand(
                Name: "",
                Description: "Desc",
                Price: 10,
                ImagePath: null,
                UserId: Guid.NewGuid());

            var validationErrors = new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Product name is required")
            });

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationErrors);

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Product name is required", ex.Message);

            _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ShouldThrowValidationException_NonPositivePrice()
        {
            // arrange
            var command = new CreateProductCommand(
                Name: "Banana",
                Description: "Desc",
                Price: 0,
                ImagePath: null,
                UserId: Guid.NewGuid());

            var validationErrors = new ValidationResult(new[]
            {
                new ValidationFailure("Price", "Price must be greater than 0")
            });

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationErrors);

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Price must be greater than 0", ex.Message);

            _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
