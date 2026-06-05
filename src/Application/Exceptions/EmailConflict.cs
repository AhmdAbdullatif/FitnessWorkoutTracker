namespace Application.Exceptions;

public class EmailConflict : Exception
{
    public EmailConflict() : base("Email is already taken.")
    {
        
    }
}
