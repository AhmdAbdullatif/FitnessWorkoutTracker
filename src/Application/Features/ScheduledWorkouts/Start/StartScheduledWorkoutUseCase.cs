using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using NodaTime.TimeZones;

namespace Application.Features.ScheduledWorkouts.Start;

public class StartScheduledWorkoutUseCase(IScheduledWorkoutRepository scheduledWorkoutRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter) : IStartScheduledWorkoutUseCase
{
    public async Task<StartScheduledWorkoutResponse> ExecuteAsync(Guid scheduledWorkoutId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");
            
        var userId = currentUserAccessor.GetId();

        var scheduledWorkout = await scheduledWorkoutRepository
            .GetByIdWithWorkoutThenExercises(scheduledWorkoutId, userId);

        if (scheduledWorkout is null)
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");

        scheduledWorkout.Start();

        var response = new StartScheduledWorkoutResponse()
        {
            Id = scheduledWorkout.Id,
            StartedAt = utcLocalConverter.ConvertUtcToLocal(scheduledWorkout.StartedAt.GetValueOrDefault(), userZone),
            Status = scheduledWorkout.Status
        };

        await scheduledWorkoutRepository.SaveChangesAsync();

        return response;
    }
}
