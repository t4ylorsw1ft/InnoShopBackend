using InnoShop.UserService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.RefreshToken)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.EmailConfirmationCodeHash) 
                .HasMaxLength(6);

            builder.Property(u => u.ResetPasswordCodeHash)
                .HasMaxLength(8);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.RefreshToken).IsUnique();
        }
    }
}
