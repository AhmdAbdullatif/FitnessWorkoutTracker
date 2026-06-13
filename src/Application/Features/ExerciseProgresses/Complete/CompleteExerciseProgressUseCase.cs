using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;

namespace Application.Features.ExerciseProgresses.Complete;

public class CompleteExerciseProgressUseCase(IExerciseProgressRepository exerciseProgressRepository,
    ICurrentUserAccessor currentUserAccessor) : ICompleteExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId)
    {
        var userId = currentUserAccessor.GetId();

        var exerciseProgress = await exerciseProgressRepository.GetByIdWithScheduledWorkout(exerciseProgressId, userId);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress with ID `{exerciseProgressId}` not found.");

        exerciseProgress.Complete();
        await exerciseProgressRepository.SaveChangesAsync();
    }
}
