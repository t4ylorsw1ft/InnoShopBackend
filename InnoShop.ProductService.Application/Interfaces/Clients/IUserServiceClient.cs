using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.Interfaces.Clients
{
    public interface IUserServiceClient
    {
        Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken);
    }
}
