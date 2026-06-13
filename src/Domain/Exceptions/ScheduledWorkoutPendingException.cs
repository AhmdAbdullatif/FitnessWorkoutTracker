namespace Domain.Exceptions;

public class ScheduledWorkoutPendingException : DomainException
{
    public ScheduledWorkoutPendingException(string message) : base(message)
    {
        
    }
}
