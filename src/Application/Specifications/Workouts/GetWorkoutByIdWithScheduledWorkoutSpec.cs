using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Workouts;

public class GetWorkoutByIdWithScheduledWorkoutSpec : Specification<Workout>
{
    public GetWorkoutByIdWithScheduledWorkoutSpec(Guid workoutId, Guid userId)
    {
        Query
            .Include(x => x.ScheduledWorkouts)
            .Where(x => x.Id == workoutId && x.UserId == userId);
    }
}
