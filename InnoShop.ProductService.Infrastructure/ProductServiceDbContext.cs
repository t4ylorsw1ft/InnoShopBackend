using InnoShop.ProductService.Domain.Entities;
using InnoShop.ProductService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;


namespace InnoShop.ProductService.Infrastructure
{
    public class ProductServiceDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
