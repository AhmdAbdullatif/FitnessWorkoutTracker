using Domain.Entities;

namespace Application.Abstraction;

public interface IScheduledWorkoutRepository
{
    Task<ScheduledWorkout?> GetByIdWithWorkoutThenExercises(Guid scheduledWorkoutId);
    Task SaveChangesAsync();
}
