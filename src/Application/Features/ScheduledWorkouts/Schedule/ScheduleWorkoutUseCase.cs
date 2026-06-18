using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.Workouts;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ScheduledWorkouts.Schedule;

public class ScheduleWorkoutUseCase(IRepository<Workout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter,
    IAppLogger<ScheduleWorkoutUseCase> logger) : IScheduleWorkoutUseCase
{
    public async Task<Guid> ExecuteAsync(DateTime sessionDate, Guid workoutId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
        {
            logger.LogDebug("Timezone information missing for scheduling workout. WorkoutId: {WorkoutId}", workoutId);
            throw new DateTimeZoneNotFoundException("");
        }

        var userId = currentUserAccessor.GetId();

        var spec = new GetWorkoutByIdWithScheduledWorkoutSpec(workoutId, userId);
        var workout = await repository.FirstOrDefaultAsync(spec);
        if (workout is null)
        {
            logger.LogInformation("Workout with ID `{WorkoutId}` not found for scheduling. UserId: {UserId}", workoutId, userId);
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");
        }

        var sessionInstant = utcLocalConverter.ConvertLocalToUtc(sessionDate, userZone);
        var scheduledWorkout = ScheduledWorkout.Schedule(workout, sessionInstant);

        workout.AddScheduledWorkout(scheduledWorkout);
        logger.LogInformation("Workout scheduled. WorkoutId: {WorkoutId}, ScheduledWorkoutId: {ScheduledWorkoutId}, SessionDate: {SessionDate}, UserId: {UserId}",
            workoutId, scheduledWorkout.Id, sessionDate, userId);

        await repository.SaveChangesAsync();

        return scheduledWorkout.Id;
    }
}
