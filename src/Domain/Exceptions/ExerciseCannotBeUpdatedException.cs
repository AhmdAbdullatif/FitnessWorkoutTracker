namespace Domain.Exceptions;

public class ExerciseCannotBeUpdatedException : DomainException
{
    public ExerciseCannotBeUpdatedException(Guid id) : base($"Exercise progress `{id}` not started yet and cannot be updated")
    {
        
    }
}
