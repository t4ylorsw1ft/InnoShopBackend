using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.Interfaces.Services;
using InnoShop.UserService.Domain.Infrastructure;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using InnoShop.UserService.Infrastructure.Repositories;
using InnoShop.UserService.Infrastructure.Security;
using InnoShop.UserService.Infrastructure.Security.Email;
using InnoShop.UserService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration["DbConnection"];
            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(connectionString));


            services.Configure<SmtpOptions>(configuration.GetSection(nameof(SmtpOptions)));

            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IEmailConfirmationCodeProvider, EmailConfirmationCodeProvider>();
            services.AddScoped<IResetPasswordCodeProvider, ResetPasswordCodeProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IJwtValidator, JwtValidator>();
            return services;
        }
    }
}
