using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Infrastructure;
using InnoShop.UserService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Tests.Unit
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly UserServiceDbContext _context;
        private readonly UserRepository _userRepository;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UserServiceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new UserServiceDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task CreateAsync_AddsUserToDb()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                RefreshToken = "test-refresh-token"
            };

            var result = await _userRepository.CreateAsync(user, _cancellationToken);

            var fromDb = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(fromDb);
            Assert.Equal(user.Email, fromDb!.Email);
            Assert.Equal(user.RefreshToken, fromDb.RefreshToken);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesUserInDb()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "original@test.com",
                RefreshToken = "original-refresh-token"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.Email = "updated@test.com";

            var updated = await _userRepository.UpdateAsync(user, _cancellationToken);

            var fromDb = await _context.Users.FindAsync(user.Id);
            Assert.Equal("updated@test.com", fromDb!.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUserOrNull()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "name",
                Email = "test@test.com",
                RefreshToken = "refresh-token"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var found = await _userRepository.GetByIdAsync(user.Id, _cancellationToken);
            var notFound = await _userRepository.GetByIdAsync(Guid.NewGuid(), _cancellationToken);

            Assert.NotNull(found);
            Assert.Equal("test@test.com", found!.Email);
            Assert.Null(notFound);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUserOrNull()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "name",
                Email = "test@test.com",
                RefreshToken = "refresh-token"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var found = await _userRepository.GetByEmailAsync(user.Email, _cancellationToken);
            var notFound = await _userRepository.GetByEmailAsync("nonexistent@test.com", _cancellationToken);

            Assert.NotNull(found);
            Assert.Equal("test@test.com", found!.Email);
            Assert.Null(notFound);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ReturnsUserOrNull()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "name",
                Email = "test@test.com",
                RefreshToken = "refresh-token"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var found = await _userRepository.GetByRefreshTokenAsync(user.RefreshToken, _cancellationToken);
            var notFound = await _userRepository.GetByRefreshTokenAsync("invalid-token", _cancellationToken);

            Assert.NotNull(found);
            Assert.Equal("refresh-token", found!.RefreshToken);
            Assert.Null(notFound);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrueWhenUserExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "name",
                Email = "test@test.com",
                RefreshToken = "refresh-token"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var exists = await _userRepository.ExistsAsync(user.Id, _cancellationToken);
            var notExists = await _userRepository.ExistsAsync(Guid.NewGuid(), _cancellationToken);

            Assert.True(exists);
            Assert.False(notExists);
        }
    }
}
