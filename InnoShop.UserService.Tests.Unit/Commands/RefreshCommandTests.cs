using FluentValidation;
using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.UseCases.Commands.Authentication.Refresh;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Tests.Unit.Commands
{
    public class RefreshCommandTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly Mock<IJwtValidator> _jwtValidatorMock;
        private readonly RefreshCommandHandler _handler;

        public RefreshCommandTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _jwtValidatorMock = new Mock<IJwtValidator>();

            _handler = new RefreshCommandHandler(
                _userRepositoryMock.Object,
                _jwtProviderMock.Object,
                _jwtValidatorMock.Object
            );
        }

        [Fact]
        public async Task RefreshCommandHandler_ValidRefreshToken_ReturnsNewJwtPair()
        {
            // arrange
            var refreshCommand = new RefreshCommand("valid_refresh_token");
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", RefreshToken = "valid_refresh_token" };
            _userRepositoryMock.Setup(repo => repo.GetByRefreshTokenAsync(refreshCommand.RefreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _jwtValidatorMock.Setup(validator => validator.ValidateTokenByExpiration(refreshCommand.RefreshToken)).Returns(true);
            _jwtProviderMock.Setup(provider => provider.GenerateAccessToken(user)).Returns("new_access_token");
            _jwtProviderMock.Setup(provider => provider.GenerateRefreshToken()).Returns("new_refresh_token");

            // act
            var result = await _handler.Handle(refreshCommand, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal("new_access_token", result.AccessToken);
            Assert.Equal("new_refresh_token", result.RefreshToken);
        }

        [Fact]
        public async Task RefreshCommandHandler_InvalidRefreshToken_ThrowsNotFoundException()
        {
            // arrange
            var refreshCommand = new RefreshCommand("invalid_refresh_token");
            _userRepositoryMock.Setup(repo => repo.GetByRefreshTokenAsync(refreshCommand.RefreshToken, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

            // act and assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _handler.Handle(refreshCommand, CancellationToken.None));
        }

        [Fact]
        public async Task RefreshCommandHandler_ExpiredRefreshToken_ThrowsException()
        {
            // arrange
            var refreshCommand = new RefreshCommand("expired_refresh_token");
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", RefreshToken = "expired_refresh_token" };
            _userRepositoryMock.Setup(repo => repo.GetByRefreshTokenAsync(refreshCommand.RefreshToken, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _jwtValidatorMock.Setup(validator => validator.ValidateTokenByExpiration(refreshCommand.RefreshToken)).Returns(false);

            // act and assert
            await Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(refreshCommand, CancellationToken.None));
        }
    }
}