namespace Domain.Exceptions;

public class ExerciseNotInProgressException : DomainException
{
    public ExerciseNotInProgressException(string message) : base(message)
    {
        
    }
}
