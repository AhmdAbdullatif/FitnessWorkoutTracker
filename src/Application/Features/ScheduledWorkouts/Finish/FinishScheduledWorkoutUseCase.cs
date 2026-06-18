using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;

namespace Application.Features.ScheduledWorkouts.Finish;

public class FinishScheduledWorkoutUseCase(IRepository<ScheduledWorkout> repository,
    ICurrentUserAccessor currentUserAccessor,
    IAppLogger<FinishScheduledWorkoutUseCase> logger) : IFinishScheduledWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetScheduledWorkoutByIdWithExerciseProgressesSpec(scheduledWorkoutId, userId);

        var scheduledWorkout = await repository
            .FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
        {
            logger.LogInformation("Scheduled workout with ID `{ScheduledWorkoutId}` not found for finishing. UserId: {UserId}", scheduledWorkoutId, userId);
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");
        }

        var completedCount = scheduledWorkout.ExerciseProgresses.Count(x => x.Status == ExerciseStatus.Completed);
        scheduledWorkout.Finish();
        logger.LogInformation("Scheduled workout finished. ScheduledWorkoutId: {ScheduledWorkoutId}, CompletedExercises: {CompletedExercises}, UserId: {UserId}",
            scheduledWorkoutId, completedCount, userId);

        await repository.SaveChangesAsync();
    }
}
