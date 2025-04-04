using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using System.Reflection;
using FluentValidation.AspNetCore;
using InnoShop.UserService.Application.UseCases.Commands.Authentication.Register;


namespace InnoShop.UserService.Application
{
    public static class DependencyInjection
    { 
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();

            return services;
        }
    }
}