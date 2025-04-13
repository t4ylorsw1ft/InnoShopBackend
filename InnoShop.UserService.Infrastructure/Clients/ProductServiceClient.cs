using InnoShop.UserService.Application.Interfaces.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Infrastructure.Clients
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DeactivateProductsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Calling: {_httpClient.BaseAddress}api/products/deactivate-user/{userId}");
            var response = await _httpClient.PutAsync(
                $"api/products/deactivate-user/{userId}",
                null,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
        }

        public async Task ActivateProductsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Calling: {_httpClient.BaseAddress}api/products/deactivate-user/{userId}");
            var response = await _httpClient.PutAsync(
                $"api/products/activate-user/{userId}",
                null,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
        }
    }
}
