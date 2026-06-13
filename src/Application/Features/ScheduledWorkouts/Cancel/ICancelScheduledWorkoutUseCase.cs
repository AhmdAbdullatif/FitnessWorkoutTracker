using Application.Features.ScheduledWorkouts.GetAll;

namespace Application.Features.ScheduledWorkouts.Cancel;

public interface ICancelScheduledWorkoutUseCase
{
    Task<ScheduledWorkoutDto> ExecuteAsync(Guid scheduledWorkoutId, string userZone);
}