using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ScheduledWorkouts;

public class GetScheduledWorkoutByIdWithWorkoutReadonlySpec
    : Specification<ScheduledWorkout>
{
    public GetScheduledWorkoutByIdWithWorkoutReadonlySpec(Guid scheduledWorkoutId, Guid userId)
    {
        Query
            .AsNoTracking()
            .Include(x => x.Workout)
            .Where(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }
}
