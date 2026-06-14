using Domain.Entities;

namespace Application.Abstraction;

public interface IScheduledWorkoutRepository
{
    Task<ScheduledWorkout?> GetByIdWithWorkoutThenExercises(Guid scheduledWorkoutId, Guid userId);
    Task<ScheduledWorkout?> GetByIdWithExerciseProgressesThenWithExercise(Guid scheduledWorkoutId, Guid userId);
    Task<IEnumerable<ScheduledWorkout>> GetAllWithWorkout(Guid workoutId, Guid userId);
    Task<ScheduledWorkout?> GetByIdWithWorkout(Guid scheduledWorkoutId, Guid userId);
    Task<ScheduledWorkout?> GetByIdWithWorkoutAndExerciseProgresses(Guid scheduledWorkoutId, Guid userId);
    Task SaveChangesAsync();
}
