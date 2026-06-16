using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.Workouts;
using Domain.Entities;

namespace Application.Features.Workouts.Delete;

public class DeleteWorkoutUseCase(IRepository<Workout> repository,
    ICurrentUserAccessor currentUserAccessor) : IDeleteWorkoutUseCase
{
    public async Task ExecuteAsync(Guid workoutId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetWorkoutByIdSpec(workoutId, userId);
        var workout = await repository.FirstOrDefaultAsync(spec);

        if (workout is null)
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");

        await repository.DeleteAsync(workout);
    }
}
