using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ScheduledWorkouts;

public class GetScheduledWorkoutByIdWithExerciseProgressesThenWithExerciseReadonlySpec
    : Specification<ScheduledWorkout>
{
    public GetScheduledWorkoutByIdWithExerciseProgressesThenWithExerciseReadonlySpec(Guid scheduledWorkoutId, Guid userId)
    {
        Query
            .AsNoTracking()
            .Include(x => x.ExerciseProgresses)
            .ThenInclude(x => x.Exercise)
            .Where(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }
}
