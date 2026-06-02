namespace Domain.Exceptions;

public class SessionDateNotInTheFutureException : Exception
{
    public SessionDateNotInTheFutureException() : base("Session date must be in the future")
    {

    }
}
