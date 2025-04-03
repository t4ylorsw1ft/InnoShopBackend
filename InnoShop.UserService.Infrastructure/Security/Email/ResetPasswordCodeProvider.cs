using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Infrastructure.Security.Email
{
    public class ResetPasswordCodeProvider
    {
        public string GenerateCode()
        {
            return new Random().Next(10000000, 99999999).ToString();
        }
    }
}
