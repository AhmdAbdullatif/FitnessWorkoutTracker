namespace Application.Exceptions;

public class InvalidUserCredentialsException : Exception
{
    public InvalidUserCredentialsException() : base("User's email address or password is wrong.")
    {
        
    }
}
