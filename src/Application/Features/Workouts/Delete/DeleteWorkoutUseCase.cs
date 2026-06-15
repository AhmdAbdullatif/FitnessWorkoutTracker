using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;

namespace Application.Features.Workouts.Delete;

public class DeleteWorkoutUseCase(IWorkoutRepository workoutRepository,
    ICurrentUserAccessor currentUserAccessor) : IDeleteWorkoutUseCase
{
    public async Task ExecuteAsync(Guid workoutId)
    {
        var userId = currentUserAccessor.GetId();

        var workout = await workoutRepository.GetByIdAsync(workoutId, userId);

        if (workout is null)
            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");

        workoutRepository.Delete(workout);
        await workoutRepository.SaveChangesAsync();
    }
}
