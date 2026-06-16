using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Workouts;

public class GetWorkoutByIdWithExercisesSpec : Specification<Workout>
{
    public GetWorkoutByIdWithExercisesSpec(Guid workoutId, Guid userId)
    {
        Query
            .Include(x => x.Exercises)
            .Where(x => x.Id == workoutId && x.UserId == userId);
    }
}
