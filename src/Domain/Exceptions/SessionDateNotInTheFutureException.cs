namespace Domain.Exceptions;

public class SessionDateNotInTheFutureException : DomainException
{
    public SessionDateNotInTheFutureException() : base("Session date must be in the future")
    {

    }
}
