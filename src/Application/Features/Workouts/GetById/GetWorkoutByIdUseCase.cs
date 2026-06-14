using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using NodaTime.TimeZones;

namespace Application.Features.Workouts.GetById;

public class GetWorkoutByIdUseCase(IWorkoutRepository workoutRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter) : IGetWorkoutByIdUseCase
{
    public async Task<GetWorkoutByIdResponse> ExecuteAsync(Guid workoutId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");

        var userId = currentUserAccessor.GetId();

        var workout = await workoutRepository.GetByIdWithOrderedScheduledWorkoutsReadOnlyAsync(workoutId, userId);

        if (workout is null)
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");

        return new GetWorkoutByIdResponse()
        {
            Id = workout.Id,
            Title = workout.Title,
            Description = workout.Description,
            CreatedAt = utcLocalConverter.ConvertUtcToLocal(workout.CreatedAt, userZone),
            ExercisesCount = workout.ExercisesCount,
            ScheduledWorkoutDtos = workout.ScheduledWorkouts
                .Select(scheduledWorkout => new ScheduledWorkoutDto()
                {
                    Id = scheduledWorkout.Id,
                    Status = scheduledWorkout.Status,
                    SessionDate = utcLocalConverter.ConvertUtcToLocal(scheduledWorkout.SessionDate, userZone),
                    StartedAt = scheduledWorkout.StartedAt is null
                        ? null
                        : utcLocalConverter.ConvertUtcToLocal(scheduledWorkout.StartedAt.Value, userZone),
                    CompletedAt = scheduledWorkout.CompletedAt is null
                        ? null
                        : utcLocalConverter.ConvertUtcToLocal(scheduledWorkout.CompletedAt.Value, userZone)
                })
        };
    }

}
