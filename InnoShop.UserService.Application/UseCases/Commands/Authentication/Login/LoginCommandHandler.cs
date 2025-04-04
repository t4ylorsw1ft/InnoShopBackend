using FluentValidation;
using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using InnoShop.UserService.Application.Interfaces.Security;
using MediatR;
using InnoShop.UserService.Application.UseCases.DTOs;

namespace InnoShop.UserService.Application.UseCases.Commands.Authentication.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, JwtPairDto>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IValidator<LoginCommand> _loginCommandValidator;

        public LoginCommandHandler(IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IJwtProvider jwtProvider,
            IValidator<LoginCommand> loginCommandValidator)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _loginCommandValidator = loginCommandValidator;
        }

        public async Task<JwtPairDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _loginCommandValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new LoginException();

            string accessToken = _jwtProvider.GenerateAccessToken(user);
            string refreshToken = _jwtProvider.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            await _userRepository.UpdateAsync(user, cancellationToken);

            JwtPairDto jwtPair = new JwtPairDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return jwtPair;
        }


    }
}
