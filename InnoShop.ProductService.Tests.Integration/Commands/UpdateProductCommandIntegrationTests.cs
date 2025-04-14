using InnoShop.ProductService.Application.Common.Exceptions;
using InnoShop.ProductService.Application.UseCases.Products.Commands.UpdateProduct;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using InnoShop.ProductService.Infrastructure.Repositories;
using InnoShop.ProductService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace InnoShop.ProductService.Tests.Integration.Commands
{
    public class UpdateProductCommandIntegrationTests : IDisposable
    {
        private readonly ProductServiceDbContext _dbContext;
        private readonly IProductRepository _productRepository;
        private readonly UpdateProductCommandHandler _handler;
        private readonly UpdateProductCommandValidator _validator;

        public UpdateProductCommandIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ProductServiceDbContext(options);
            _productRepository = new ProductRepository(_dbContext);
            _validator = new UpdateProductCommandValidator();
            _handler = new UpdateProductCommandHandler(_productRepository, _validator);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldUpdateProductInDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalProduct = new Product
            {
                Name = "Original Product",
                Description = "Original Description",
                Price = 19.99,
                IsAvaliable = false,
                ImagePath = "original.jpg",
                UserId = userId,
                CreationDateTime = DateTime.UtcNow
            };

            await _dbContext.Products.AddAsync(originalProduct);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateProductCommand(
                ProductId: originalProduct.Id,
                Name: "Updated Product",
                Description: "Updated Description",
                Price: 29.99,
                IsAvailable: true,
                ImagePath: "updated.jpg",
                UserId: userId);

            // act
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Price, result.Price);
            Assert.Equal(command.IsAvailable, result.IsAvaliable);
            Assert.Equal(command.ImagePath, result.ImagePath);
            Assert.Equal(userId, result.UserId);

            var updatedProduct = await _dbContext.Products.FindAsync(originalProduct.Id);
            Assert.Equal(command.Name, updatedProduct.Name);
            Assert.Equal(command.Description, updatedProduct.Description);
            Assert.Equal(command.Price, updatedProduct.Price);
            Assert.Equal(command.IsAvailable, updatedProduct.IsAvaliable);
            Assert.Equal(command.ImagePath, updatedProduct.ImagePath);
            Assert.Equal(originalProduct.CreationDateTime, updatedProduct.CreationDateTime);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowNotFoundException()
        {
            // arrange
            var command = new UpdateProductCommand(
                ProductId: Guid.NewGuid(),
                Name: "Test Product",
                Description: "Test Description",
                Price: 19.99,
                IsAvailable: true,
                ImagePath: null,
                UserId: Guid.NewGuid());

            // act and assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WrongUserId_ShouldThrowAccessException()
        {
            // arrange
            var originalOwnerId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();

            var originalProduct = new Product
            {
                Name = "Original Product",
                UserId = originalOwnerId
            };

            await _dbContext.Products.AddAsync(originalProduct);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateProductCommand(
                ProductId: originalProduct.Id,
                Name: "Updated Product",
                Description: "Updated Description",
                Price: 29.99,
                IsAvailable: true,
                ImagePath: "updated.jpg",
                UserId: differentUserId);

            // act and assert
            await Assert.ThrowsAsync<AccessException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidName_ShouldThrowValidationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalProduct = new Product
            {
                Name = "Original Product",
                UserId = userId
            };

            await _dbContext.Products.AddAsync(originalProduct);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateProductCommand(
                ProductId: originalProduct.Id,
                Name: "",
                Description: "Updated Description",
                Price: 29.99,
                IsAvailable: true,
                ImagePath: "updated.jpg",
                UserId: userId);

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains("Product name is required", ex.Message);
            var dbProduct = await _dbContext.Products.FindAsync(originalProduct.Id);
            Assert.Equal("Original Product", dbProduct.Name);
        }

        [Fact]
        public async Task Handle_NullImagePath_ShouldUpdateSuccessfully()
        {
            // arrange
            var userId = Guid.NewGuid();
            var originalProduct = new Product
            {
                Name = "Original Product",
                ImagePath = "original.jpg",
                UserId = userId
            };

            await _dbContext.Products.AddAsync(originalProduct);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateProductCommand(
                ProductId: originalProduct.Id,
                Name: "Updated Product",
                Description: "Updated Description",
                Price: 29.99,
                IsAvailable: true,
                ImagePath: null, 
                UserId: userId);

            // act
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.Null(result.ImagePath);
            var updatedProduct = await _dbContext.Products.FindAsync(originalProduct.Id);
            Assert.Null(updatedProduct.ImagePath);
        }
    }
}
