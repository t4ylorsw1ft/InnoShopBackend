using InnoShop.UserService.Application.Interfaces.Security;
using InnoShop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InnoShop.UserService.Infrastructure.Security
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;

        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateAccessToken(User user)
        {
            Claim[] claims =
            [
                new("userId", user.Id.ToString()),
                new("Role", user.Role.ToString())
            ];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddHours(_options.ExpiresHours)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(accessToken);

            return token;
        }

        public string GenerateRefreshToken()
        {
            Claim[] claims =
            [
                new("body", Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))),
            ];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var refreshToken = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddDays(_options.RefreshExpiresDays)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            return token;
        }
    }
}
