using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ExerciseProgresses;

public class GetExerciseProgressByIdSpec
    : Specification<ExerciseProgress>
{
    public GetExerciseProgressByIdSpec(Guid exerciseProgressId, Guid userId)
    {
        Query
            .Where(x => x.Id == exerciseProgressId &&
                x.ScheduledWorkout!.Workout!.UserId == userId);
    }
}
