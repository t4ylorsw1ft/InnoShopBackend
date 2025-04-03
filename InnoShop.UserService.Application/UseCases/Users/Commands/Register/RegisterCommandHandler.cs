using AutoMapper;
using FluentValidation;
using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.UseCases.Users.DTOs;
using MediatR;
using InnoShop.UserService.Domain.Entities;

namespace Library.Application.UseCases.Users.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, JwtPairDto>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterCommand> _registerCommandValidator;

        public RegisterCommandHandler(IPasswordHasher passwordHasher, 
            IUserRepository userRepository, 
            IJwtProvider jwtProvider, 
            IMapper mapper, 
            IValidator<RegisterCommand> registerCommandValidator)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
            _registerCommandValidator = registerCommandValidator;
        }

        public async Task<JwtPairDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var validationResult = await _registerCommandValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            string username = request.Username;
            string email = request.Email;
            string passwordHash = _passwordHasher.Generate(request.Password);

            if (await _userRepository.GetByEmailAsync(email, cancellationToken) != null)
                throw new AlreadyExistsException("email");

            User user = new User()
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash
            };

            var newUser = await _userRepository.CreateAsync(user, cancellationToken);

            string accessToken = _jwtProvider.GenerateAccessToken(newUser);
            string refreshToken = _jwtProvider.GenerateRefreshToken();

            newUser.RefreshToken = refreshToken;
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
