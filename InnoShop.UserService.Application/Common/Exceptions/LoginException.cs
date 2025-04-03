namespace InnoShop.UserService.Application.Common.Exceptions
{
    public class LoginException : Exception
    {
        public LoginException()
            : base($"Invalid email or password")
        {

        }
    }
}
