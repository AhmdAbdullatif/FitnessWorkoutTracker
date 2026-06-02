namespace Domain.Exceptions;

public class WorkoutWithoutExercisesException : Exception
{
    public Guid WorkoutId { get; }
    public WorkoutWithoutExercisesException(Guid workoutId)
            : base($"Workout {workoutId} has no exercises and cannot be scheduled.")
    {
        WorkoutId = workoutId;
    }
}
