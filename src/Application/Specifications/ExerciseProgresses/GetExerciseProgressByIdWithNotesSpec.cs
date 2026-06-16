using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ExerciseProgresses;

public class GetExerciseProgressByIdWithNotesSpec : Specification<ExerciseProgress>
{
    public GetExerciseProgressByIdWithNotesSpec(Guid exerciseProgressId, Guid userId)
    {
        Query
            .Include(x => x.Notes)
            .Where(x => x.Id == exerciseProgressId && x.ScheduledWorkout!.Workout!.UserId == userId);
    }
}
