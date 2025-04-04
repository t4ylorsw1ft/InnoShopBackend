using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.Interfaces.Services;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Commands.PasswordReset.SendResetPasswordEmail
{
    public class SendResetPasswordEmailCommandHandler : IRequestHandler<SendResetPasswordEmailCommand>
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

        public async Task Handle(SendResetPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                return; //не исключение чтобы нельзя было перебрать email

            if (user.ResetSendDateTime > DateTime.UtcNow.AddMinutes(-5))
                throw new Exception($"Too many password reset requests. Try again in {(DateTime.UtcNow - user.ConfirmationSendDateTime.AddMinutes(5)).TotalMinutes:F0} minutes");

            user.ResetSendDateTime = DateTime.UtcNow;

            var resetCode = _resetPasswordCodeProvider.GenerateCode();
            user.ResetPasswordCodeHash = _passwordHasher.Generate(resetCode);

            await _userRepository.UpdateAsync(user, cancellationToken);

            await _emailService.SendEmailAsync(user.Email, "Innoshop email verification", $"Your verification code: {resetCode}", cancellationToken);
        }
    }
}
