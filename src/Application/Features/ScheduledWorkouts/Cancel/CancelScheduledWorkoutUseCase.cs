using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;

namespace Application.Features.ScheduledWorkouts.Cancel;

public class CancelScheduledWorkoutUseCase(IRepository<ScheduledWorkout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IAppLogger<CancelScheduledWorkoutUseCase> logger) : ICancelScheduledWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetScheduledWorkoutByIdWithExerciseProgressesSpec(scheduledWorkoutId, userId);

        var scheduledWorkout = await repository.FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
        {
            logger.LogInformation("Scheduled workout with ID `{ScheduledWorkoutId}` not found for cancellation. UserId: {UserId}",
                scheduledWorkoutId,
                userId);
                
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");
        }

        scheduledWorkout.Cancel();
        logger.LogInformation("Scheduled workout cancelled. ScheduledWorkoutId: {ScheduledWorkoutId}, UserId: {UserId}", scheduledWorkoutId, userId);

        await repository.SaveChangesAsync();
    }
}
