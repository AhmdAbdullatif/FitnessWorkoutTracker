namespace Application.Exceptions;

public class EmailConflictException : Exception
{
    public EmailConflictException() : base("Email is already taken.")
    {
        
    }
}
