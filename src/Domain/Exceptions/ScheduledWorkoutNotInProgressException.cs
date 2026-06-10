namespace Domain.Exceptions;

public class ScheduledWorkoutNotInProgressException : DomainException
{
    public ScheduledWorkoutNotInProgressException(Guid scheduledWorkoutId) : base ($"Scheduled workout {scheduledWorkoutId} not in in-progress status.")
    {
        
    }
}
