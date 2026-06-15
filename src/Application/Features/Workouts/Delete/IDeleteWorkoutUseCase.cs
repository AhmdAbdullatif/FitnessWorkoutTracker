namespace Application.Features.Workouts.Delete;

public interface IDeleteWorkoutUseCase
{
    Task ExecuteAsync(Guid workoutId);
}