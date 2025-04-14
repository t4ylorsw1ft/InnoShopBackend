using InnoShop.ProductService.Application.Common.Exceptions;
using InnoShop.ProductService.Application.UseCases.Products.Queries.GetProductById;
using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop_ProductService.Tests.Unit.Queries
{
    public class GetProductByIdQueryTests
    {
        [Fact]
        public async Task Handle_ProductExists_ShouldReturnProduct()
        {
            // arrange
            var productId = Guid.NewGuid();
            var expectedProduct = new Product
            {
                Id = productId,
                Name = "apple",
                Description = "desc",
                Price = 99.99,
                IsAvaliable = true,
                UserId = Guid.NewGuid(),
                CreationDateTime = DateTime.UtcNow,
                ImagePath = null,
                IsActive = true
            };

            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock
                .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProduct);

            var handler = new GetProductByIdQueryHandler(repositoryMock.Object);
            var query = new GetProductByIdQuery(productId);

            // act
            var result = await handler.Handle(query, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.Id, result.Id);
            Assert.Equal("apple", result.Name);
            repositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductDoesNotExist_ShouldThrowNotFoundException()
        {
            // arrange
            var productId = Guid.NewGuid();

            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock
                .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new GetProductByIdQueryHandler(repositoryMock.Object);
            var query = new GetProductByIdQuery(productId);

            // act and assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, CancellationToken.None));

            repositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
