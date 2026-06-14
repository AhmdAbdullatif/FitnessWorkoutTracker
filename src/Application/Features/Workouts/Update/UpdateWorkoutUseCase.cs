using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;

namespace Application.Features.Workouts.Update;

public class UpdateWorkoutUseCase(IWorkoutRepository workoutRepository,
    ICurrentUserAccessor currentUserAccessor) : IUpdateWorkoutUseCase
{
    public async Task ExecuteAsync(Guid workoutId, UpdateWorkoutRequest req)
    {
        var userId = currentUserAccessor.GetId();

        var workout = await workoutRepository.GetById(workoutId, userId);

        if (workout is null)
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");

        workout.UpdateDetails(req.Title, req.Description);

        await workoutRepository.SaveChangesAsync();
    }
}
