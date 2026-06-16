using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications.ScheduledWorkouts;

public class GetScheduledWorkoutByIdWithExerciseProgressesSpec
    : Specification<ScheduledWorkout>
{
    public GetScheduledWorkoutByIdWithExerciseProgressesSpec(Guid scheduledWorkoutId, Guid userId)
    {
        Query
            .Include(x => x.ExerciseProgresses)
            .Where(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }
}
