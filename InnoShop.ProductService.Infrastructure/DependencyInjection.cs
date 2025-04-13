using InnoShop.ProductService.Application.Interfaces.Services;
using InnoShop.ProductService.Domain.Interfaces;
using InnoShop.ProductService.Infrastructure.Repositories;
using InnoShop.ProductService.Infrastructure.Security;
using InnoShop.ProductService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.ProductService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration["DbConnection"];
            services.AddDbContext<ProductServiceDbContext>(
                options => options.UseSqlServer(connectionString));

            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
