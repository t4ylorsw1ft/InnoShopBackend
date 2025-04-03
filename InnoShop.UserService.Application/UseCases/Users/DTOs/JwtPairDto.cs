namespace InnoShop.UserService.Application.UseCases.Users.DTOs
{
    public class JwtPairDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
