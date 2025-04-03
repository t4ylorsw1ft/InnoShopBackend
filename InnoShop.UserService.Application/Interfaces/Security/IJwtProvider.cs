using InnoShop.UserService.Domain.Entities;

namespace InnoShop.UserService.Application.Interfaces.Security
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
