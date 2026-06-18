using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ExerciseProgresses.GetAll;

public class GetExerciseProgressesUseCase(IReadRepository<ScheduledWorkout> readRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter,
    IAppLogger<GetExerciseProgressesUseCase> logger) : IGetExerciseProgressesUseCase
{
    public async Task<GetExerciseProgressesResponse> ExecuteAsync(Guid scheduledWorkoutId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
        {
            logger.LogDebug("Timezone information missing for retrieving exercise progresses.");
            throw new DateTimeZoneNotFoundException("");
        }

        var userId = currentUserAccessor.GetId();

        var spec =
            new GetScheduledWorkoutByIdWithExerciseProgressesThenWithExerciseReadonlySpec(scheduledWorkoutId, userId);

        var scheduledWorkout = await readRepository
            .FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
        {
            logger.LogInformation("Scheduled workout with ID `{ScheduledWorkoutId}` not found. UserId: {UserId}",
                scheduledWorkoutId,
                userId);
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");
        }

        var exerciseProgressDtos = new List<ExerciseProgressDto>();
        foreach (var exerciseProgress in scheduledWorkout.ExerciseProgresses)
        {
            var exerciseProgressDto = new ExerciseProgressDto()
            {
                Id = exerciseProgress.Id,
                Title = exerciseProgress.Exercise!.Title,
                Description = exerciseProgress.Exercise.Description,
                Sets = exerciseProgress.Sets,
                Reps = exerciseProgress.Reps,
                Status = exerciseProgress.Status
            };

            if (exerciseProgress.StartedAt is not null)
                exerciseProgressDto.StartedAt = utcLocalConverter
                    .ConvertUtcToLocal(exerciseProgress.StartedAt.GetValueOrDefault(), userZone);

            if (exerciseProgress.CompletedAt is not null)
                exerciseProgressDto.CompletedAt = utcLocalConverter
                    .ConvertUtcToLocal(exerciseProgress.CompletedAt.GetValueOrDefault(), userZone);

            exerciseProgressDtos.Add(exerciseProgressDto);
        }

        logger.LogInformation("Retrieved {ExerciseProgressCount} exercise progresses for scheduled workout {ScheduledWorkoutId}. UserId: {UserId}",
            exerciseProgressDtos.Count, scheduledWorkoutId, userId);

        return new GetExerciseProgressesResponse()
        {
            ExerciseProgressDtos = exerciseProgressDtos
        };
    }
}
