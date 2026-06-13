using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;

namespace Application.Features.ExerciseProgresses.Skip;

public class SkipExerciseProgressUseCase(IExerciseProgressRepository exerciseProgressRepository,
    ICurrentUserAccessor currentUserAccessor) : ISkipExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId)
    {
        var userId = currentUserAccessor.GetId();

        var exerciseProgress = await exerciseProgressRepository.GetByIdWithScheduledWorkout(exerciseProgressId, userId);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress `{exerciseProgressId}` not found.");

        exerciseProgress.Skip();
        await exerciseProgressRepository.SaveChangesAsync();
    }
}
