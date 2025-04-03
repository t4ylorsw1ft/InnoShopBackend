using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.Interfaces.Services;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;


namespace InnoShop.UserService.Application.UseCases.Users.Commands.SendVerificationEmail
{
    public class SendVerificationEmailCommandHandler : IRequestHandler<SendVerificationEmailCommand>
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordCodeProvider _emailConfirmationCodeProvider;
        private readonly IPasswordHasher _passwordHasher;

        public SendVerificationEmailCommandHandler(IEmailService emailService, IUserRepository userRepository, 
            IResetPasswordCodeProvider emailConfirmationTokenProvider, IPasswordHasher passwordHasher) 
        { 
            _emailService = emailService;
            _userRepository = userRepository;
            _emailConfirmationCodeProvider = emailConfirmationTokenProvider;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException(typeof(User), request.UserId);

            if (user.ConfirmationSendDateTime > DateTime.UtcNow.AddMinutes(-1))
                throw new Exception($"Too many verification requests. Try again in {(user.ConfirmationSendDateTime.AddMinutes(1) - DateTime.UtcNow).TotalSeconds:F0} seconds");

            user.ConfirmationSendDateTime = DateTime.UtcNow;

            var confirmationToken = _emailConfirmationCodeProvider.GenerateCode();

            user.EmailConfirmationCodeHash = _passwordHasher.Generate(confirmationToken);

            await _userRepository.UpdateAsync(user, cancellationToken);

            await _emailService.SendEmailAsync(user.Email, "Innoshop email verification", $"Your verification code: {confirmationToken}", cancellationToken);
        }
    }
}
