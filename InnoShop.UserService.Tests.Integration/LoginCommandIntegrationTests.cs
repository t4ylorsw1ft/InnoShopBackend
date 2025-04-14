using FluentValidation;
using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.UseCases.Commands.Authentication.Login;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Infrastructure;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using InnoShop.UserService.Infrastructure.Repositories;
using InnoShop.UserService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Tests.Integration
{
    public class LoginCommandIntegrationTests : IDisposable
    {
        private readonly UserServiceDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly LoginCommandValidator _validator;
        private readonly LoginCommandHandler _handler;

        public LoginCommandIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<UserServiceDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _dbContext = new UserServiceDbContext(options);
            _userRepository = new UserRepository(_dbContext);
            _passwordHasher = new PasswordHasher();

            var jwtOptions = Options.Create(new JwtOptions
            {
                SecretKey = "TestSecretKeyWithMinimum32CharactersLength",
                ExpiresHours = 1,
                RefreshExpiresDays = 7
            });
            _jwtProvider = new JwtProvider(jwtOptions);

            _validator = new LoginCommandValidator();
            _handler = new LoginCommandHandler(
                _passwordHasher,
                _userRepository,
                _jwtProvider,
                _validator);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task Handle_ValidCredentials_ShouldReturnValidJwtPair()
        {
            // arrange
            var email = "test@example.com";
            var password = "Password123";
            var hashedPassword = _passwordHasher.Generate(password);

            var user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                Username = "testuser"
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new LoginCommand(email, password);

            // act
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);

            var accessTokenHandler = new JwtSecurityTokenHandler();
            var accessToken = accessTokenHandler.ReadJwtToken(result.AccessToken);
            Assert.Contains(accessToken.Claims, c => c.Type == "userId" && c.Value == user.Id.ToString());

            var refreshTokenHandler = new JwtSecurityTokenHandler();
            var refreshToken = refreshTokenHandler.ReadJwtToken(result.RefreshToken);
            Assert.NotNull(refreshToken);

            var dbUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.Equal(result.RefreshToken, dbUser.RefreshToken);
        }


        [Fact]
        public async Task Handle_NonexistentUser_ShouldThrowLoginException()
        {
            // arrange
            var command = new LoginCommand("nonexistent@example.com", "Password123");

            // act and assert
            await Assert.ThrowsAsync<LoginException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidEmailFormat_ShouldThrowValidationException()
        {
            // arrange
            var command = new LoginCommand("invalid-email", "Password123");

            // act & assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains("Invalid email format", ex.Message);
        }
    }
}