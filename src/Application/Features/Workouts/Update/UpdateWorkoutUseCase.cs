using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.Workouts;
using Domain.Entities;

namespace Application.Features.Workouts.Update;

public class UpdateWorkoutUseCase(IRepository<Workout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IAppLogger<UpdateWorkoutUseCase> logger) : IUpdateWorkoutUseCase
{
    public async Task ExecuteAsync(Guid workoutId, UpdateWorkoutRequest req)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetWorkoutByIdSpec(workoutId, userId);
        var workout = await repository.FirstOrDefaultAsync(spec);

        if (workout is null)
        {
            logger.LogInformation("Workout not found.\nWorkoutId: {WorkoutId}",
                workoutId);

            throw new NotFoundException($"Workout with ID `{workoutId}` not found.");
        }

        workout.UpdateDetails(req.Title, req.Description);

        logger.LogInformation("Workout updated\nWorkoutId: {WorkoutId}\nUserId: {UserId}",
            workoutId,
            userId);

        await repository.SaveChangesAsync();
    }
}
