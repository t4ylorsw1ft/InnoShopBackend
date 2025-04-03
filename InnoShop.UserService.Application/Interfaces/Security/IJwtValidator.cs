namespace InnoShop.UserService.Application.Interfaces.Security
{
    public interface IJwtValidator
    {
        bool ValidateTokenByExpiration(string token);
    }
}