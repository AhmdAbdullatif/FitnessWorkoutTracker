using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.Workouts;
using Domain.Entities;

namespace Application.Features.Workouts.Update;

public class UpdateWorkoutUseCase(IRepository<Workout> repository,
    ICurrentUserAccessor currentUserAccessor) : IUpdateWorkoutUseCase
{
    public async Task ExecuteAsync(Guid workoutId, UpdateWorkoutRequest req)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetWorkoutByIdSpec(workoutId, userId);
        var workout = await repository.FirstOrDefaultAsync(spec);

        if (workout is null)
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");

        workout.UpdateDetails(req.Title, req.Description);

        await repository.SaveChangesAsync();
    }
}
