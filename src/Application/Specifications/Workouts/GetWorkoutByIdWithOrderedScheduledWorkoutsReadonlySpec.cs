using System.Text.RegularExpressions;
using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Workouts;

public class GetWorkoutByIdWithOrderedScheduledWorkoutsReadonlySpec : Specification<Workout>
{
    public GetWorkoutByIdWithOrderedScheduledWorkoutsReadonlySpec(Guid workoutId, Guid userId)
    {
        Query
            .AsNoTracking()
            .Include(x => x.ScheduledWorkouts.OrderBy(sw => sw.SessionDate))
            .Where(x => x.Id == workoutId && x.UserId == userId);
    }
}
