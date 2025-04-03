namespace InnoShop.UserService.Application.Interfaces.Security
{
    public interface IPasswordHasher
    {
        string Generate(string password);
        bool Verify(string password, string hashedPassword);
    }
}
