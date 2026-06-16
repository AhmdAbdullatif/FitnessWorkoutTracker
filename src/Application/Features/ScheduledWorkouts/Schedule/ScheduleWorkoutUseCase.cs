using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.Workouts;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ScheduledWorkouts.Schedule;

public class ScheduleWorkoutUseCase(IRepository<Workout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter) : IScheduleWorkoutUseCase
{
    public async Task<Guid> ExecuteAsync(DateTime sessionDate, Guid workoutId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");
            
        var userId = currentUserAccessor.GetId();

        var spec = new GetWorkoutByIdWithScheduledWorkoutSpec(workoutId, userId);
        var workout = await repository.FirstOrDefaultAsync(spec);
        if (workout is null)
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");

        var sessionInstant = utcLocalConverter.ConvertLocalToUtc(sessionDate, userZone);
        var scheduledWorkout = ScheduledWorkout.Schedule(workout, sessionInstant);

        workout.AddScheduledWorkout(scheduledWorkout);
        await repository.SaveChangesAsync();

        return scheduledWorkout.Id;
    }
}
