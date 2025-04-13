using FluentValidation;
using InnoShop.ProductService.Application.UseCases.Products.Commands.CreateProduct;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InnoShop.ProductService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

            return services;
        }
    }
}
