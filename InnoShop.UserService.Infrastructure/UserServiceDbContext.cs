using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.UserService.Domain.Infrastructure
{
    public class UserServiceDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}