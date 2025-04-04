using InnoShop.UserService.Application.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Infrastructure.Security.Email
{
    public class EmailConfirmationCodeProvider : IEmailConfirmationCodeProvider
    {
        public string GenerateCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
