using Innoshop.ProductService.Presentation.Extensions;
using InnoShop.ProductService.Application;
using InnoShop.ProductService.Application.Common.Mapping;
using InnoShop.ProductService.Application.Interfaces.Clients;
using InnoShop.ProductService.Infrastructure;
using InnoShop.ProductService.Infrastructure.Clients;
using InnoShop.ProductService.Infrastructure.Security;
using InnoShop.ProductService.Presentation.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

builder.Services.AddMemoryCache();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile(new AssemblyMappingProfile(Assembly.Load("InnoShop.ProductService.Application")));
});

var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
builder.Services.AddApiAuthentication(Options.Create(jwtOptions));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("Role", "Admin");
    });
});

builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://userservice:8080");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductServiceDbContext>();
    dbContext.Database.Migrate();
}

app.Run();