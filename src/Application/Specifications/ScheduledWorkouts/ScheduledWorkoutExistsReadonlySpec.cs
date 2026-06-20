using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ScheduledWorkouts;

public class ScheduledWorkoutExistsReadonlySpec : Specification<ScheduledWorkout>
{
    public ScheduledWorkoutExistsReadonlySpec(Guid scheduledWorkoutId, Guid userId)
    {
        Query  
            .AsNoTracking()
            .Where(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }
}
