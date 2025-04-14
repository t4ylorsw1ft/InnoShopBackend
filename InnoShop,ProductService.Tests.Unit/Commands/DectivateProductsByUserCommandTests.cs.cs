using InnoShop.ProductService.Application.UseCases.Products.Commands.DeactivateProductsByUser;
using InnoShop.ProductService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop_ProductService.Tests.Unit.Commands
{
    public class DeactivateProductsByUserCommandTests
    {
        [Fact]
        public async Task Handle_ValidCommand_ShouldCallRepositoryMethod()
        {
            // arrange
            var userId = Guid.NewGuid();
            var command = new DeactivateProductsByUserCommand(userId);
            var repositoryMock = new Mock<IProductRepository>();
            var handler = new DeactivateProductsByUserCommandHandler(repositoryMock.Object);

            // act
            await handler.Handle(command, CancellationToken.None);

            // assert
            repositoryMock.Verify(
                r => r.DeactivateByUserIdAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}