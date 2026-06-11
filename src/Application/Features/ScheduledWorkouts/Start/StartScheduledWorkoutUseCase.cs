using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;

namespace Application.Features.ScheduledWorkouts.Start;

public class StartScheduledWorkoutUseCase(IScheduledWorkoutRepository scheduledWorkoutRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter)
{
    public async Task<StartScheduledWorkoutResponse> ExecuteAsync(Guid scheduledWorkoutId, string userZone)
    {
        var scheduledWorkout = await scheduledWorkoutRepository
            .GetByIdWithWorkoutThenExercises(scheduledWorkoutId);

        var userId = currentUserAccessor.GetId();
        if (scheduledWorkout is null || scheduledWorkout.Workout!.UserId != userId)
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");

        scheduledWorkout.Start();
        await scheduledWorkoutRepository.SaveChangesAsync();

        return new StartScheduledWorkoutResponse()
        {
            Id = scheduledWorkout.Id,
            StartedAt = utcLocalConverter.ConvertUtcToLocal(scheduledWorkout.StartedAt.GetValueOrDefault(), userZone),
            Status = scheduledWorkout.Status
        };
    }
}
