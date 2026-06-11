using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Domain.Entities;

namespace Application.Features.ScheduledWorkouts.Schedule;

public class ScheduleWorkoutUseCase(IWorkoutRepository workoutRepository, 
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter)
{
    public async Task<ScheduleWorkoutResponse> ExecuteAsync(DateTime sessionDate, Guid workoutId, string userZone)
    {
        var userId = currentUserAccessor.GetId();

        var workout = await workoutRepository.GetByIdWithScheduledWorkoutsAsync(workoutId, userId);
        if (workout is null)
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");

        var sessionInstant = utcLocalConverter.ConvertLocalToUtc(sessionDate, userZone);
        var scheduledWorkout = ScheduledWorkout.Schedule(workout, sessionInstant);

        workout.AddScheduledWorkout(scheduledWorkout);
        await workoutRepository.SaveChangesAsync();

        return new ScheduleWorkoutResponse()
        {
            Id = scheduledWorkout.Id,
            SessionDate = sessionDate,
            Status = scheduledWorkout.Status
        };
    }
}
