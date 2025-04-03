using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.Interfaces.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string userEmail, string subject, string body, CancellationToken cancellationToken);
    }
}
