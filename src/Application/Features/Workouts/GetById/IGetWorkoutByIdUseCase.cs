namespace Application.Features.Workouts.GetById;

public interface IGetWorkoutByIdUseCase
{
    Task<GetWorkoutByIdResponse> ExecuteAsync(Guid workoutId, string userZone);
}