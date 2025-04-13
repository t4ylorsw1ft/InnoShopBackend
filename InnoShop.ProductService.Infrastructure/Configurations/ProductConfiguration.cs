using InnoShop.ProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(6, 2);

            builder.Property(p => p.IsAvaliable)
                .IsRequired();

            builder.Property(p => p.CreationDateTime)
                .IsRequired();

            builder.Property(p => p.ImagePath)
                .HasMaxLength(500);
        }
    }
}
