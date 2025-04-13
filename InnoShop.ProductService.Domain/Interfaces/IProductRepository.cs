using InnoShop.ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken);
        Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken);
        Task DeleteAsync(Product product , CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetAllAsync(string? name, int? minPrice, int? maxPrice, bool? isAvailable, Guid? userId, CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task DeactivateByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task ActivateByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
