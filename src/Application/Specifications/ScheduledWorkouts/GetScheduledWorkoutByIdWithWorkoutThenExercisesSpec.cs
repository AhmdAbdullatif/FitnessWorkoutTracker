using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ScheduledWorkouts;

public class GetScheduledWorkoutByIdWithWorkoutThenExercisesSpec
    : Specification<ScheduledWorkout>
{
    public GetScheduledWorkoutByIdWithWorkoutThenExercisesSpec(Guid scheduledWorkoutId, Guid userId)
    {
        Query
            .Include(x => x.Workout)
            .ThenInclude(x => x!.Exercises)
            .Where(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }
}
