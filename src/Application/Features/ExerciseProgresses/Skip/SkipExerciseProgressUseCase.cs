using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Skip;

public class SkipExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor) : ISkipExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdWithScheduledWorkoutSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress `{exerciseProgressId}` not found.");

        exerciseProgress.Skip();
        await repository.SaveChangesAsync();
    }
}
