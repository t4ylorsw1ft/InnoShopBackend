using AutoMapper;
using InnoShop.ProductService.Application.UseCases.Products.DTOs;
using InnoShop.ProductService.Application.UseCases.Products.Queries.GetAllProducts;
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
    public class GetAllProductsQueryTests
    {
        [Fact]
        public async Task Handle_ValidQuery_ShouldReturnMappedProducts()
        {
            // arrange
            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "apple",
                    Description = "desc",
                    Price = 100,
                    IsAvaliable = true,
                    UserId = Guid.NewGuid(),
                    CreationDateTime = DateTime.UtcNow,
                    ImagePath = null,
                    IsActive = true
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "banana",
                    Description = "desc",
                    Price = 200,
                    IsAvaliable = false,
                    UserId = Guid.NewGuid(),
                    CreationDateTime = DateTime.UtcNow,
                    ImagePath = null,
                    IsActive = true
                }
            };

            var productDtos = products.Select(p => new ProductLookupDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                IsAvaliable = p.IsAvaliable
            }).ToList();

            var repositoryMock = new Mock<IProductRepository>();
            var mapperMock = new Mock<IMapper>();

            repositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<string?>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<bool?>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mapperMock
                .Setup(m => m.Map<List<ProductLookupDto>>(products))
                .Returns(productDtos);

            var query = new GetAllProductsQuery(null, null, null, null, null);
            var handler = new GetAllProductsQueryHandler(repositoryMock.Object, mapperMock.Object);

            // act
            var result = await handler.Handle(query, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("apple", result[0].Name);
            Assert.Equal("banana", result[1].Name);

            repositoryMock.Verify(r => r.GetAllAsync(null, null, null, null, null, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<List<ProductLookupDto>>(products), Times.Once);
        }
    }
}
