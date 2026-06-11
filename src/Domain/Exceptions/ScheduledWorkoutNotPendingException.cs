namespace Domain.Exceptions;

public class ScheduledWorkoutNotPendingException : DomainException
{
    public ScheduledWorkoutNotPendingException(Guid Id) : base($"Scheduled workout `{Id}` not in pending status.")
    {
        
    }
}
