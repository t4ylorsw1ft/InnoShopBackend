using InnoShop.ProductService.Application.UseCases.Products.Commands.ActivateProductsByUser;
using InnoShop.ProductService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop_ProductService.Tests.Unit.Commands
{
    public class ActivateProductsByUserCommandTests
    {
        [Fact]
        public async Task Handle_ValidCommand_ShouldCallRepositoryMethod()
        {
            // arrange
            var userId = Guid.NewGuid();
            var command = new ActivateProductsByUserCommand(userId);
            var repositoryMock = new Mock<IProductRepository>();
            var handler = new ActivateProductsByUserCommandHandler(repositoryMock.Object);

            // act
            await handler.Handle(command, CancellationToken.None);

            // assert
            repositoryMock.Verify(
                r => r.ActivateByUserIdAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
