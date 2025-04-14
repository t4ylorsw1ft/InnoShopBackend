using InnoShop.ProductService.Application.Common.Exceptions;
using InnoShop.ProductService.Application.UseCases.Products.Commands.DeleteProduct;
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
    public class DeleteProductCommandTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new DeleteProductCommandHandler(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldDeleteProduct()
        {
            // arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                UserId = userId,
                Name = "Apple",
                Description = "Desc",
                Price = 10,
                IsActive = true,
                IsAvaliable = true
            };

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _productRepositoryMock
                .Setup(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new DeleteProductCommand(productId, userId);

            // act
            await _handler.Handle(command, CancellationToken.None);

            // assert
            _productRepositoryMock.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowNotFoundException()
        {
            // arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var command = new DeleteProductCommand(productId, userId);

            // act and assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            _productRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ProductNotOwnedByUser_ShouldThrowAccessException()
        {
            // arrange
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                UserId = anotherUserId,
                Name = "Apple",
                Description = "Desc",
                Price = 5,
                IsActive = true,
                IsAvaliable = true
            };

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var command = new DeleteProductCommand(productId, userId);

            // act and assert
            await Assert.ThrowsAsync<AccessException>(() => _handler.Handle(command, CancellationToken.None));

            _productRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
