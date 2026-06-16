using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Workouts;

public class GetWorkoutByIdSpec : Specification<Workout>
{
    public GetWorkoutByIdSpec(Guid workoutId, Guid userId)
    {
        Query
            .Where(x => x.Id == workoutId && x.UserId == userId);
    }
}
