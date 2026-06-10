namespace Domain.Exceptions;

public class NegativeNumberException : DomainException
{
    public NegativeNumberException(string message) : base(message)
    {

    }
}
