using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ScheduledWorkouts.Reschedule;

public class RescheduleWorkoutUseCase(IRepository<ScheduledWorkout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter,
    IAppLogger<RescheduleWorkoutUseCase> logger) : IRescheduleWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId, string userZone, DateTime sessionDate)
    {
        if (string.IsNullOrWhiteSpace(userZone))
        {
            logger.LogDebug("Timezone information missing for rescheduling workout. ScheduledWorkoutId: {ScheduledWorkoutId}", scheduledWorkoutId);
            throw new DateTimeZoneNotFoundException("");
        }

        var userId = currentUserAccessor.GetId();

        var spec = new GetScheduledWorkoutByIdSpec(scheduledWorkoutId, userId);

        var scheduledWorkout = await repository.FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
        {
            logger.LogInformation("Scheduled workout with ID `{ScheduledWorkoutId}` not found for rescheduling. UserId: {UserId}", scheduledWorkoutId, userId);
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");
        }

        var sessionInstant = utcLocalConverter.ConvertLocalToUtc(sessionDate, userZone);
        scheduledWorkout.Reschedule(sessionInstant);
        logger.LogInformation("Scheduled workout rescheduled. ScheduledWorkoutId: {ScheduledWorkoutId}, NewSessionDate: {NewSessionDate}, UserId: {UserId}",
            scheduledWorkoutId, sessionDate, userId);

        await repository.SaveChangesAsync();
    }
}
