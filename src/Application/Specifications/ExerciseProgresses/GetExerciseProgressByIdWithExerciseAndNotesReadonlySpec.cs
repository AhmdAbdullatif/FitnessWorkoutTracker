
using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ExerciseProgresses;

public class GetExerciseProgressByIdWithExerciseAndNotesReadonlySpec
    : Specification<ExerciseProgress>
{
    public GetExerciseProgressByIdWithExerciseAndNotesReadonlySpec(Guid exerciseProgressId, Guid userId)
    {
        Query
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Exercise)
            .Include(x => x.Notes)
            .Where(x => x.Id == exerciseProgressId
            && x.ScheduledWorkout!.Workout!.UserId == userId);
    }
}
