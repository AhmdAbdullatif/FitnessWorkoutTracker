using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ExerciseProgresses.Update;

public class UpdateExerciseProgressStatusUseCase(IExerciseProgressRepository exerciseProgressRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter)
{
    public async Task<UpdateExerciseProgressStatusResponse> ExecuteAsync(Guid exerciseProgressId, string userZone, UpdateExerciseProgressStatusRequest req)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");

        var userId = currentUserAccessor.GetId();

        var exerciseProgress = await exerciseProgressRepository.GetByIdWithScheduledWorkout(exerciseProgressId, userId);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress with ID `{exerciseProgressId}` not found.");

        exerciseProgress.UpdateStatus(req.Status);
        var response = new UpdateExerciseProgressStatusResponse()
        {
            Id = exerciseProgress.Id,
            Status = exerciseProgress.Status,
            StartedAt = utcLocalConverter.ConvertUtcToLocal(exerciseProgress.StartedAt.GetValueOrDefault(), userZone),
            CompletedAt = utcLocalConverter.ConvertUtcToLocal(exerciseProgress.CompletedAt.GetValueOrDefault(), userZone)
        };

        await exerciseProgressRepository.SaveChangesAsync();
        
        return response;
    }
}
