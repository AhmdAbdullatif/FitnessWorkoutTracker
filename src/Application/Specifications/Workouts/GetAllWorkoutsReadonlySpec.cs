using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.Workouts;

public class GetAllWorkoutsReadonlySpec : Specification<Workout>
{
    public GetAllWorkoutsReadonlySpec(Guid userId)
    {
        Query
            .AsNoTracking()
            .Where(x => x.UserId == userId);
    }
}
