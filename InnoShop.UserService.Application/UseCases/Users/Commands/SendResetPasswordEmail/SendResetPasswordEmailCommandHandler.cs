using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.Interfaces.Services;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Users.Commands.SendResetPasswordEmail
{
    public class SendResetPasswordEmailCommandHandler
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordCodeProvider _resetPasswordCodeProvider;
        private readonly IPasswordHasher _passwordHasher;

        public SendResetPasswordEmailCommandHandler(IEmailService emailService, IUserRepository userRepository, 
            IResetPasswordCodeProvider resetPasswordCodeProvider, IPasswordHasher passwordHasher)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _resetPasswordCodeProvider = resetPasswordCodeProvider;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                return; //не исключение чтобы нельзя было перебрать email

            if (user.ResetSendDateTime > DateTime.UtcNow.AddMinutes(-5))
                throw new Exception($"Too many verification requests. Try again in {(user.ConfirmationSendDateTime.AddMinutes(5) - DateTime.UtcNow).TotalMinutes:F0} minutes");

            user.ResetSendDateTime = DateTime.UtcNow;

            var resetCode = _resetPasswordCodeProvider.GenerateCode();
            user.EmailConfirmationCodeHash = _passwordHasher.Generate(resetCode);

            await _userRepository.UpdateAsync(user, cancellationToken);

            await _emailService.SendEmailAsync(user.Email, "Innoshop email verification", $"Your verification code: {resetCode}", cancellationToken);
        }
    }
}
