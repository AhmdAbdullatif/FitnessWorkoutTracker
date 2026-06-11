namespace Domain.Exceptions;

public class ExerciseNotPendingException : DomainException
{
    public ExerciseNotPendingException(Guid exerciseId) : base($"Exercise progress `{exerciseId}` not in pending status.")
    {
        
    }
}
