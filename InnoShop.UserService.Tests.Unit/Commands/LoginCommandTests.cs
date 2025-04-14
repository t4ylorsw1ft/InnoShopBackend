using FluentValidation;
using FluentValidation.Results;
using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.UseCases.Commands.Authentication.Login;
using InnoShop.UserService.Application.UseCases.DTOs;
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
    public class LoginCommandTests
    {
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly Mock<IValidator<LoginCommand>> _validatorMock;
        private readonly LoginCommandHandler _handler;

        public LoginCommandTests()
        {
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _validatorMock = new Mock<IValidator<LoginCommand>>();

            _handler = new LoginCommandHandler(
                _passwordHasherMock.Object,
                _userRepositoryMock.Object,
                _jwtProviderMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnJwtPair()
        {
            // arrange
            var command = new LoginCommand(
                Email: "test@example.com",
                Password: "password123");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = command.Email,
                PasswordHash = "hashed_password",
                RefreshToken = null
            };

            var expectedJwtPair = new JwtPairDto
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.Verify(command.Password, user.PasswordHash))
                .Returns(true);

            _jwtProviderMock
                .Setup(j => j.GenerateAccessToken(user))
                .Returns(expectedJwtPair.AccessToken);

            _jwtProviderMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns(expectedJwtPair.RefreshToken);

            // act
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedJwtPair.AccessToken, result.AccessToken);
            Assert.Equal(expectedJwtPair.RefreshToken, result.RefreshToken);

            _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ShortPassword_ShouldThrowValidationException()
        {
            // arrange
            var command = new LoginCommand(
                Email: "test@example.com",
                Password: "short");

            var validationErrors = new ValidationResult(new[]
            {
                new ValidationFailure("Password", "Password must be at least 8 characters long")
            });

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationErrors);

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Password must be at least 8 characters long", ex.Message);

            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowLoginException()
        {
            // arrange
            var command = new LoginCommand(
                Email: "nonexistent@example.com",
                Password: "password123");

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // act and assert
            await Assert.ThrowsAsync<LoginException>(() => _handler.Handle(command, CancellationToken.None));

            _passwordHasherMock.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WrongPassword_ShouldThrowLoginException()
        {
            // arrange
            var command = new LoginCommand(
                Email: "test@example.com",
                Password: "wrong_password");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = command.Email,
                PasswordHash = "hashed_password",
                RefreshToken = null
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.Verify(command.Password, user.PasswordHash))
                .Returns(false);

            // act and assert
            await Assert.ThrowsAsync<LoginException>(() => _handler.Handle(command, CancellationToken.None));

            _jwtProviderMock.Verify(j => j.GenerateAccessToken(It.IsAny<User>()), Times.Never);
        }
    }
}
