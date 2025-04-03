using MimeKit;
using InnoShop.UserService.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using InnoShop.UserService.Infrastructure.Security.Email;

namespace InnoShop.UserService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _smtpOptions;

        public EmailService(IOptions<SmtpOptions> smtpSettings)
        {
            _smtpOptions = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string userEmail, string subject, string body, CancellationToken cancellationToken)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
            message.To.Add(new MailboxAddress("", userEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpOptions.Server, _smtpOptions.Port, false);
                await client.AuthenticateAsync(_smtpOptions.SenderEmail, _smtpOptions.Password);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}
