using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ScheduledWorkouts.Reschedule;

public class RescheduleWorkoutUseCase(IRepository<ScheduledWorkout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter) : IRescheduleWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId, string userZone, DateTime sessionDate)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");

        var userId = currentUserAccessor.GetId();

        var spec = new GetScheduledWorkoutByIdSpec(scheduledWorkoutId, userId);

        var scheduledWorkout = await repository.FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");

        var sessionInstant = utcLocalConverter.ConvertLocalToUtc(sessionDate, userZone);
        scheduledWorkout.Reschedule(sessionInstant);

        await repository.SaveChangesAsync();
    }
}
