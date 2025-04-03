using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.Interfaces.Services;
using InnoShop.UserService.Application.UseCases.Users.Commands.VerifyEmail;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Users.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordCommandHandler(IEmailService emailService, IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException(typeof(User), request.UserId);

            if (user.ConfirmationSendDateTime.AddMinutes(10) > DateTime.UtcNow)
                throw new Exception("Code lifetime has expired");

            if (request.NewPassword != request.NewPasswordConfirmation)
                throw new Exception("Passwords must match");

            if (_passwordHasher.Verify(request.ResetPasswordCode, user.ResetPasswordCodeHash))
            {
                user.PasswordHash = _passwordHasher.Generate(request.NewPassword);
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
            else
                throw new Exception("Invalid code");
        }
    }
}
