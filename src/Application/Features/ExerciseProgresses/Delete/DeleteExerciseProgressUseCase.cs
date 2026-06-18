using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Delete;

public class DeleteExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor,
    IAppLogger<DeleteExerciseProgressUseCase> logger) : IDeleteExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
        {
            logger.LogInformation("Exercise progress with ID `{ExerciseProgressId}` not found for deletion. UserId: {UserId}",
                exerciseProgressId,
                userId);
            throw new NotFoundException($"Exercise progress `{exerciseProgressId}` not found.");
        }

        await repository.DeleteAsync(exerciseProgress);
        logger.LogInformation("Exercise progress deleted. ExerciseProgressId: {ExerciseProgressId}, UserId: {UserId}", exerciseProgressId, userId);
    }
}
