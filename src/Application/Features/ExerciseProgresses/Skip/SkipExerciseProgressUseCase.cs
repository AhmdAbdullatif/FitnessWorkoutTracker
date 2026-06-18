using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Skip;

public class SkipExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor,
    IAppLogger<SkipExerciseProgressUseCase> logger) : ISkipExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdWithScheduledWorkoutSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
        {
            logger.LogInformation("Exercise progress with ID `{ExerciseProgressId}` not found for skipping. UserId: {UserId}",
                exerciseProgressId,
                userId);
            throw new NotFoundException($"Exercise progress `{exerciseProgressId}` not found.");
        }

        exerciseProgress.Skip();
        logger.LogInformation("Exercise progress skipped. ExerciseProgressId: {ExerciseProgressId}, UserId: {UserId}", exerciseProgressId, userId);

        await repository.SaveChangesAsync();
    }
}
