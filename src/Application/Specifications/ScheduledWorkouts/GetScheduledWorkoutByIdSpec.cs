using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ScheduledWorkouts;

public class GetScheduledWorkoutByIdSpec : Specification<ScheduledWorkout>
{
    public GetScheduledWorkoutByIdSpec(Guid scheduledWorkoutId, Guid userId)
    {
        Query
            .Where(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }
}
