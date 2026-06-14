namespace Application.Features.Workouts.Update;

public interface IUpdateWorkoutUseCase
{
    Task ExecuteAsync(Guid workoutId, UpdateWorkoutRequest req);
}