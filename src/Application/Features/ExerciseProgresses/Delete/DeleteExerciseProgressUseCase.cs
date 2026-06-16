using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Delete;

public class DeleteExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor) : IDeleteExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress `{exerciseProgressId}` not found.");

        await repository.DeleteAsync(exerciseProgress);
    }
}
