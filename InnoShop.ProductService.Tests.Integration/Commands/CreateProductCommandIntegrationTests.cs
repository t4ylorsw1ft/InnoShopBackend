using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using InnoShop.ProductService.Application.UseCases.Products.Commands.CreateProduct;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using InnoShop.ProductService.Infrastructure.Repositories;
using InnoShop.ProductService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InnoShop.ProductService.Tests.Integration.Commands
{
    public class CreateProductCommandIntegrationTests : IDisposable
    {
        private readonly ProductServiceDbContext _dbContext;
        private readonly IProductRepository _productRepository;
        private readonly CreateProductCommandHandler _handler;
        private readonly IValidator<CreateProductCommand> _validator;

        public CreateProductCommandIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ProductServiceDbContext(options);
            _productRepository = new ProductRepository(_dbContext);
            _validator = new CreateProductCommandValidator();
            _handler = new CreateProductCommandHandler(_productRepository, _validator);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateProductInDatabase()
        {
            // arrange
            var userId = Guid.NewGuid();
            var command = new CreateProductCommand(
                Name: "Test Product",
                Description: "Test Description",
                Price: 19.99,
                ImagePath: "test.jpg",
                UserId: userId);

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

            var dbProduct = await _dbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(dbProduct);
            Assert.Equal(command.Name, dbProduct.Name);
            Assert.Equal(command.Description, dbProduct.Description);
            Assert.Equal(command.Price, dbProduct.Price);
            Assert.Equal(command.UserId, dbProduct.UserId);
        }

        [Fact]
        public async Task Handle_InvalidName_ShouldThrowValidationException()
        {
            // arrange
            var command = new CreateProductCommand(
                Name: "", // Invalid empty name
                Description: "Test Description",
                Price: 19.99,
                ImagePath: "test.jpg",
                UserId: Guid.NewGuid());

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains("Product name is required", ex.Message);

            var dbProduct = await _dbContext.Products.FirstOrDefaultAsync();
            Assert.Null(dbProduct);
        }

        [Fact]
        public async Task Handle_InvalidPrice_ShouldThrowValidationException()
        {
            // arrange
            var command = new CreateProductCommand(
                Name: "Test Product",
                Description: "Test Description",
                Price: 0, // Invalid price
                ImagePath: "test.jpg",
                UserId: Guid.NewGuid());

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains("Price must be greater than 0", ex.Message);

            var dbProduct = await _dbContext.Products.FirstOrDefaultAsync();
            Assert.Null(dbProduct);
        }

        [Fact]
        public async Task Handle_MultipleProducts_ShouldCreateAllWithUniqueIds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command1 = new CreateProductCommand(
                Name: "Product 1",
                Description: "Description 1",
                Price: 10.99,
                ImagePath: "image1.jpg",
                UserId: userId);

            var command2 = new CreateProductCommand(
                Name: "Product 2",
                Description: "Description 2",
                Price: 20.99,
                ImagePath: "image2.jpg",
                UserId: userId);

            // act
            var result1 = await _handler.Handle(command1, CancellationToken.None);
            var result2 = await _handler.Handle(command2, CancellationToken.None);

            // assert
            Assert.NotEqual(result1.Id, result2.Id);

            var dbProducts = await _dbContext.Products.ToListAsync();
            Assert.Equal(2, dbProducts.Count);
            Assert.Contains(dbProducts, p => p.Name == "Product 1");
            Assert.Contains(dbProducts, p => p.Name == "Product 2");
        }
    }
}