namespace Application.Exceptions;

public class InvalidUserCredentials : Exception
{
    public InvalidUserCredentials() : base("User email or password are wrong.")
    {
        
    }
}
