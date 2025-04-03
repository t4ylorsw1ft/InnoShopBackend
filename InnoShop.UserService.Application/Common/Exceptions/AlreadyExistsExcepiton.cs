namespace InnoShop.UserService.Application.Common.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException()
            : base("Such record already exists")
        {

        }

        public AlreadyExistsException(string keyName)
            : base($"Record with such {keyName} already exists")
        {

        }
    }
}
