using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Application.Specifications.ScheduledWorkouts;
using Domain.Entities;

namespace Application.Features.ScheduledWorkouts.Delete;

public class DeleteScheduledWorkoutUseCase(IRepository<ScheduledWorkout> repository,
    ICurrentUserAccessor currentUserAccessor) : IDeleteScheduledWorkoutUseCase
{
    public async Task ExecuteAsync(Guid scheduledWorkoutId)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetScheduledWorkoutByIdSpec(scheduledWorkoutId, userId);

        var scheduledWorkout = await repository.FirstOrDefaultAsync(spec);

        if (scheduledWorkout is null)
            throw new NotFoundException($"Scheduled workout with ID `{scheduledWorkoutId}` not found.");

        await repository.DeleteAsync(scheduledWorkout);
    }
}
