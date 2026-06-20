using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ExerciseProgresses;

public class GetExerciseProgressesWithExerciseReadonlySpec : Specification<ExerciseProgress>
{
    public GetExerciseProgressesWithExerciseReadonlySpec(Guid scheduledWorkoutId, Guid userId)
    {
        Query
            .AsNoTracking()
            .Include(x => x.Exercise)
            .Where(x => x.ScheduledWorkoutId == scheduledWorkoutId && x.ScheduledWorkout!.Workout!.UserId == userId);
    }
}
