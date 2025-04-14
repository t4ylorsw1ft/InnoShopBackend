using FluentValidation;
using FluentValidation.Results;
using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.UseCases.Commands.Authentication.Register;
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
    public class RegisterCommandTests
    {
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly Mock<IValidator<RegisterCommand>> _validatorMock;
        private readonly RegisterCommandHandler _handler;

        public RegisterCommandTests()
        {
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _validatorMock = new Mock<IValidator<RegisterCommand>>();

            _handler = new RegisterCommandHandler(
                _passwordHasherMock.Object,
                _userRepositoryMock.Object,
                _jwtProviderMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldRegisterUserAndReturnJwtPair()
        {
            // arrange
            var command = new RegisterCommand(
                Username: "testuser",
                Email: "test@example.com",
                Password: "Password123");

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                Username = command.Username,
                Email = command.Email,
                PasswordHash = "hashed_password",
                RefreshToken = "refresh_token" // Add this line
            };

            var expectedJwtPair = new JwtPairDto
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _passwordHasherMock
                .Setup(h => h.Generate(command.Password))
                .Returns("hashed_password");

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Setup CreateAsync to return a user with RefreshToken set
            _userRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User u, CancellationToken _) => {
                    u.RefreshToken = expectedJwtPair.RefreshToken;
                    return u;
                });

            _jwtProviderMock
                .Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
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

            _userRepositoryMock.Verify(r => r.CreateAsync(It.Is<User>(u =>
                u.Username == command.Username &&
                u.Email == command.Email &&
                u.PasswordHash == "hashed_password"),
                It.IsAny<CancellationToken>()), Times.Once);

            // Verify UpdateAsync was called with any User that has the correct RefreshToken
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.RefreshToken == expectedJwtPair.RefreshToken),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidPassword_ShouldThrowValidationException()
        {
            // arrange
            var command = new RegisterCommand(
                Username: "testuser",
                Email: "test@example.com",
                Password: "weak");

            var validationErrors = new ValidationResult(new[]
            {
                new ValidationFailure("Password", "Password must be at least 8 characters long"),
                new ValidationFailure("Password", "Password must contain at least one uppercase letter"),
                new ValidationFailure("Password", "Password must contain at least one digit")
            });

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationErrors);

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Password must be at least 8 characters long", ex.Message);
            Assert.Contains("Password must contain at least one uppercase letter", ex.Message);
            Assert.Contains("Password must contain at least one digit", ex.Message);

            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidUsername_ShouldThrowValidationException()
        {
            // arrange
            var command = new RegisterCommand(
                Username: "a", // Too short
                Email: "test@example.com",
                Password: "Password123");

            var validationErrors = new ValidationResult(new[]
            {
                new ValidationFailure("Username", "Username must be at least 3 characters long")
            });

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationErrors);

            // act and assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Username must be at least 3 characters long", ex.Message);

            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}