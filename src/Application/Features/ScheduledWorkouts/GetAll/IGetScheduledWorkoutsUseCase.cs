namespace Application.Features.ScheduledWorkouts.GetAll;

public interface IGetScheduledWorkoutsUseCase
{
    Task<GetScheduledWorkoutsResponse> ExecuteAsync(GetScheduledWorkoutsQuery req, Guid workoutId, string userZone);
}