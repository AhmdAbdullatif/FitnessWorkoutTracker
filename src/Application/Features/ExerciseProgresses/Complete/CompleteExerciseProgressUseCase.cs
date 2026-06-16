using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Complete;

public class CompleteExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor) : ICompleteExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdWithScheduledWorkoutSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress with ID `{exerciseProgressId}` not found.");

        exerciseProgress.Complete();
        await repository.SaveChangesAsync();
    }
}
