using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;

namespace Application.Features.ScheduledWorkouts.Start;

public class StartScheduledWorkoutUseCase(IRepository<ScheduledWorkout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IAppLogger<StartScheduledWorkoutUseCase> logger) : IStartScheduledWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetScheduledWorkoutByIdWithWorkoutThenExercisesSpec(scheduledWorkoutId, userId);
        var scheduledWorkout = await repository.FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
        {
            logger.LogInformation("Scheduled workout with ID `{ScheduledWorkoutId}` not found for starting. UserId: {UserId}", scheduledWorkoutId, userId);
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");
        }

        scheduledWorkout.Start();
        logger.LogInformation("Scheduled workout started. ScheduledWorkoutId: {ScheduledWorkoutId}, ExerciseCount: {ExerciseCount}, UserId: {UserId}",
            scheduledWorkoutId, scheduledWorkout.ExerciseProgresses.Count, userId);

        await repository.SaveChangesAsync();
    }
}
