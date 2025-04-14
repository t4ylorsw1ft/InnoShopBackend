using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Infrastructure;
using InnoShop.ProductService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoShop_ProductService.Tests.Unit
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly ProductServiceDbContext _context;
        private readonly ProductRepository _productRepository;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ProductServiceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ProductServiceDbContext(options);
            _productRepository = new ProductRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task CreateAsync_AddsProductToDb()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Price = 100,
                IsAvaliable = true,
                IsActive = true,
                UserId = Guid.NewGuid()
            };

            var result = await _productRepository.CreateAsync(product, _cancellationToken);

            var fromDb = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(fromDb);
            Assert.Equal(product.Name, fromDb!.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesProductInDb()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Original",
                Price = 100,
                IsAvaliable = true,
                IsActive = true,
                UserId = Guid.NewGuid()
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            product.Name = "Updated";

            var updated = await _productRepository.UpdateAsync(product, _cancellationToken);

            var fromDb = await _context.Products.FindAsync(product.Id);
            Assert.Equal("Updated", fromDb!.Name);
        }

        [Fact]
        public async Task DeleteAsync_RemovesProductFromDb()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "DeleteMe",
                Price = 50,
                IsAvaliable = false,
                IsActive = false,
                UserId = Guid.NewGuid()
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _productRepository.DeleteAsync(product, _cancellationToken);

            var fromDb = await _context.Products.FindAsync(product.Id);
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFilteredResults()
        {
            var userId = Guid.NewGuid();

            var products = new[]
            {
                new Product { Id = Guid.NewGuid(), Name = "Book", Price = 10, IsAvaliable = true, IsActive = true, UserId = userId },
                new Product { Id = Guid.NewGuid(), Name = "Phone", Price = 500, IsAvaliable = false, IsActive = true, UserId = userId }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            var results = await _productRepository.GetAllAsync("Book", null, null, true, userId, _cancellationToken);

            Assert.Single(results);
            Assert.Contains(results, p => p.Name == "Book");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProductOrNull()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Price = 900,
                IsAvaliable = true,
                IsActive = true,
                UserId = Guid.NewGuid()
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var found = await _productRepository.GetByIdAsync(product.Id, _cancellationToken);
            var notFound = await _productRepository.GetByIdAsync(Guid.NewGuid(), _cancellationToken);

            Assert.NotNull(found);
            Assert.Equal("Laptop", found!.Name);
            Assert.Null(notFound);
        }

        [Fact]
        public async Task DeactivateByUserIdAsync_SetsIsActiveToFalse()
        {
            var userId = Guid.NewGuid();

            var product = new Product { Id = Guid.NewGuid(), Name = "Old", IsActive = true, UserId = userId };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _productRepository.DeactivateByUserIdAsync(userId, _cancellationToken);

            var fromDb = await _context.Products.FindAsync(product.Id);
            Assert.False(fromDb!.IsActive);
        }

        [Fact]
        public async Task ActivateByUserIdAsync_SetsIsActiveToTrue()
        {
            var userId = Guid.NewGuid();

            var product = new Product { Id = Guid.NewGuid(), Name = "Inactive", IsActive = false, UserId = userId };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _productRepository.ActivateByUserIdAsync(userId, _cancellationToken);

            var fromDb = await _context.Products.FindAsync(product.Id);
            Assert.True(fromDb!.IsActive);
        }
    }
}