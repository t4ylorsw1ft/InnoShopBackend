using Innoshop.UserService.Presentation.Extensions;
using InnoShop.UserService.Application;
using InnoShop.UserService.Domain.Infrastructure;
using InnoShop.UserService.Infrastructure;
using InnoShop.UserService.Infrastructure.Security;
using InnoShop.UserService.Presentation.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
builder.Services.AddApiAuthentication(Options.Create(jwtOptions));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("Role", "Admin");
    });
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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
