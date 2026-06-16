using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ExerciseProgresses;

public class GetExerciseProgressByIdWithScheduledWorkoutSpec
    : Specification<ExerciseProgress>
{
    public GetExerciseProgressByIdWithScheduledWorkoutSpec(Guid exerciseProgressId, Guid userId)
    {
        Query
            .Include(x => x.ScheduledWorkout)
            .Where(x => x.Id == exerciseProgressId &&
                x.ScheduledWorkout!.Workout!.UserId == userId);
    }
}
