namespace Domain.Exceptions;

public class ExerciseProgressCannotBeCanceledException : DomainException
{
    public ExerciseProgressCannotBeCanceledException() : base("Cannot skip a completed exercise progress.")
    {
        
    }
}
