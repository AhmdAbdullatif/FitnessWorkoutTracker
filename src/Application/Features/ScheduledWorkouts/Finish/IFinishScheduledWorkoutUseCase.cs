using Application.Features.ScheduledWorkouts.GetAll;

namespace Application.Features.ScheduledWorkouts.Finish;

public interface IFinishScheduledWorkoutUseCase
{
    Task<ScheduledWorkoutDto> ExecuteAsync(Guid scheduledWorkoutId, string userZone);
}