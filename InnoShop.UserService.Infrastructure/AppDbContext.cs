using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Infrastructure.Configurations;
using InnoShop.UserService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace InnoShop.UserService.Domain.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}