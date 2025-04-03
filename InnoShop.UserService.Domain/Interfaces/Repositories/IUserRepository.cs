using InnoShop.UserService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        public Task<User?> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken);
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        public Task<User> CreateAsync(User user, CancellationToken cancellationToken);
        public Task<User> UpdateAsync(User user, CancellationToken cancellationToken);
    }
}
