using InnoShop.ProductService.Application.Interfaces.Clients;
using System.Net.Http.Json;

namespace InnoShop.ProductService.Infrastructure.Clients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"api/users/active-status/{userId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);
        }
    }
}
