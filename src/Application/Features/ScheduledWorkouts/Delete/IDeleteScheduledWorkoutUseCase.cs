namespace Application.Features.ScheduledWorkouts.Delete;

public interface IDeleteScheduledWorkoutUseCase
{
    Task ExecuteAsync(Guid scheduledWorkoutId);
}