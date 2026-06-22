using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ScheduledWorkouts;
using Application.Specifications.Workouts;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ScheduledWorkouts.GetAll;

public class GetScheduledWorkoutsUseCase(
    IReadRepository<Workout> workoutRepository,
    IReadRepository<ScheduledWorkout> scheduledWorkoutRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter,
    IAppLogger<GetScheduledWorkoutsUseCase> logger) : IGetScheduledWorkoutsUseCase
{
    public async Task<GetScheduledWorkoutsResponse> ExecuteAsync(GetScheduledWorkoutsQuery query,
        Guid workoutId,
        string userZone
    )
    {
        if (string.IsNullOrWhiteSpace(userZone))
        {
            logger.LogDebug("Timezone information missing for retrieving scheduled workouts. WorkoutId: {WorkoutId}",
                workoutId);

            throw new DateTimeZoneNotFoundException("");
        }

        var userId = currentUserAccessor.GetId();

        var workoutSpec = new GetWorkoutByIdReadonlySpec(workoutId, userId);
        var workoutExists = await workoutRepository.AnyAsync(workoutSpec);
        if (!workoutExists)
        {
            logger.LogInformation("Workout not found for retrieving scheduled workouts.\nWorkoutId: {WorkoutId}",
                workoutId);

            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");
        }

        var scheduledWorkoutsSpec = new GetScheduledWorkoutsReadonlySpec(
            workoutId,
            userId,
            query.SortOrder?.Trim().ToLower(),
            skip: (query.Page - 1) * query.PageSize,
            take: query.PageSize
        );

        var scheduledWorkouts = await scheduledWorkoutRepository.ListAsync(scheduledWorkoutsSpec);
        var response = new GetScheduledWorkoutsResponse()
        {
            ScheduledWorkoutDtos = scheduledWorkouts.Select(x => new ScheduledWorkoutDto
            {
                Id = x.Id,
                Status = x.Status,
                SessionDate = utcLocalConverter.ConvertUtcToLocal(x.SessionDate, userZone),
                StartedAt = x.StartedAt is null
                        ? null
                        : utcLocalConverter.ConvertUtcToLocal(x.StartedAt.Value, userZone),
                CompletedAt = x.CompletedAt is null
                        ? null
                        : utcLocalConverter.ConvertUtcToLocal(x.CompletedAt.Value, userZone)
            })
        };

        return response;
    }
}
