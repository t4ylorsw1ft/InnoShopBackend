using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductServiceDbContext _context;
        
        public ProductRepository(ProductServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken)
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? name, int? minPrice, int? maxPrice, bool? isAvailable, Guid? userId, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (isAvailable.HasValue)
                query = query.Where(p => p.IsAvaliable == isAvailable);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task DeactivateByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var products = await _context.Products
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                product.IsActive = false;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ActivateByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var products = await _context.Products
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                product.IsActive = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
