using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Domain.Entities
{
    public enum UserRole
    {
        User,
        Admin
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public bool IsEmailConfirmed { get; set; } = false;

        public string? EmailConfirmationCodeHash { get; set; }
        public DateTime ConfirmationSendDateTime { get; set; }

        public string? ResetPasswordCodeHash {  get; set; }
        public DateTime ResetSendDateTime { get; set; }
    }
}
