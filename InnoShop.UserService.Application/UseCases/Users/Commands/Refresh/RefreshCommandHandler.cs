using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Application.UseCases.Users.DTOs;
using MediatR;
using InnoShop.UserService.UseCases.Users.Commands.Refresh;
using InnoShop.UserService.Domain.Entities;

namespace Library.Application.UseCases.Users.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, JwtPairDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IJwtValidator _jwtValidator;

        public RefreshCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider, IJwtValidator jwtValidator)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _jwtValidator = jwtValidator;
        }

        public async Task<JwtPairDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            string refreshToken = request.RefreshToken;

            User? user = await _userRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);

            if (user == null)
                throw new NotFoundException(typeof(User), refreshToken);

            if (!_jwtValidator.ValidateTokenByExpiration(refreshToken))
                throw new Exception("Token has expired");

            string newAccessToken = _jwtProvider.GenerateAccessToken(user);
            string newRefreshToken = _jwtProvider.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateAsync(user, cancellationToken);

            JwtPairDto jwtPair = new JwtPairDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return jwtPair;
        }
    }
}
