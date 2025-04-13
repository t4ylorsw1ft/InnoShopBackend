using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.Interfaces.Clients
{
    public interface IProductServiceClient
    {
        Task DeactivateProductsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task ActivateProductsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
