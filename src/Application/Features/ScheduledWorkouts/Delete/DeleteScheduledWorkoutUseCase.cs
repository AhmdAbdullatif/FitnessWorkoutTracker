using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;

namespace Application.Features.ScheduledWorkouts.Delete;

public class DeleteScheduledWorkoutUseCase(IScheduledWorkoutRepository scheduledWorkoutRepository,
    ICurrentUserAccessor currentUserAccessor) : IDeleteScheduledWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId)
    {
        var userId = currentUserAccessor.GetId();

        var scheduledWorkout = await scheduledWorkoutRepository.GetByIdAsync(scheduledWorkoutId, userId);

        if (scheduledWorkout is null)
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");

        scheduledWorkoutRepository.Delete(scheduledWorkout);
        await scheduledWorkoutRepository.SaveChangesAsync();
    }
}
