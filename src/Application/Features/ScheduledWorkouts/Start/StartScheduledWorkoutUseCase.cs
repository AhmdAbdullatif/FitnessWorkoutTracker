using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;

namespace Application.Features.ScheduledWorkouts.Start;

public class StartScheduledWorkoutUseCase(IRepository<ScheduledWorkout> repository,
    ICurrentUserAccessor currentUserAccessor) : IStartScheduledWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetScheduledWorkoutByIdWithWorkoutThenExercisesSpec(scheduledWorkoutId, userId);
        var scheduledWorkout = await repository.FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");

        scheduledWorkout.Start();

        await repository.SaveChangesAsync();
    }
}
