using Application.Abstraction;
using Application.Exceptions;
using Application.Features.ScheduledWorkouts.GetAll;
using Application.Features.Workouts.Create;
using NodaTime.TimeZones;

namespace Application.Features.ScheduledWorkouts.Cancel;

public class CancelScheduledWorkoutUseCase(IScheduledWorkoutRepository scheduledWorkoutRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter) : ICancelScheduledWorkoutUseCase
{
    public async Task<ScheduledWorkoutDto> ExecuteAsync(Guid scheduledWorkoutId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");

        var userId = currentUserAccessor.GetId();

        var scheduledWorkout = await scheduledWorkoutRepository
            .GetByIdWithWorkoutAndExerciseProgresses(scheduledWorkoutId, userId);

        if (scheduledWorkout is null)
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");

        scheduledWorkout.Cancel();
        
        var scheduledWorkoutDto = new ScheduledWorkoutDto()
        {
            Id = scheduledWorkout.Id,
            Title = scheduledWorkout.Workout!.Title,
            Description = scheduledWorkout.Workout!.Description,
            Status = scheduledWorkout.Status,
            SessionDate = utcLocalConverter.ConvertUtcToLocal(scheduledWorkout.SessionDate, userZone),
            StartedAt = utcLocalConverter.ConvertUtcToLocal(scheduledWorkout.StartedAt.GetValueOrDefault(), userZone),
        };

        await scheduledWorkoutRepository.SaveChangesAsync();

        return scheduledWorkoutDto;
    }

}
