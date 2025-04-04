using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.Interfaces.Security
{
    public interface IEmailConfirmationCodeProvider
    {
        public string GenerateCode();
    }
}
