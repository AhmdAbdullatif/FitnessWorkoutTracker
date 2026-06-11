using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;

namespace Application.Features.ExerciseProgresses.Start;

public class StartExerciseProgressUseCase(IExerciseProgressRepository exerciseProgressRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter)
{
    public async Task<StartExerciseResponse> ExecuteAsync(Guid exerciseProgressId,
        StartExerciseRequest request,
        string userZone)
    {
        var userId = currentUserAccessor.GetId();

        var exerciseProgress = await exerciseProgressRepository
            .GetByIdWithScheduledWorkout(exerciseProgressId, userId);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress `{exerciseProgressId}` not found.");

        exerciseProgress.Start(request.Sets, request.Reps);
        await exerciseProgressRepository.SaveChangesAsync();

        return new StartExerciseResponse()
        {
            Id = exerciseProgressId,
            StartedAt = utcLocalConverter
                .ConvertUtcToLocal(exerciseProgress.StartedAt.GetValueOrDefault(), userZone),

            Sets = exerciseProgress.Sets,
            Reps = exerciseProgress.Reps,
            Status = exerciseProgress.Status
        };
    }
}
