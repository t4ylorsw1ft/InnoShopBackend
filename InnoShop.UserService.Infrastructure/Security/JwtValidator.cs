using InnoShop.UserService.Application.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Security
{
    public class JwtValidator : IJwtValidator
    {
        public bool ValidateTokenByExpiration(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token);
            return securityToken.ValidTo > DateTime.UtcNow;
        }
    }
}
