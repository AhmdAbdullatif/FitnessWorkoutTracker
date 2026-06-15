using Domain.Entities;

namespace Application.Abstraction;

public interface IScheduledWorkoutRepository
{
    Task<ScheduledWorkout?> GetByIdWithWorkoutThenExercises(Guid scheduledWorkoutId, Guid userId);
    Task<ScheduledWorkout?> GetByIdWithExerciseProgressesThenWithExercise(Guid scheduledWorkoutId, Guid userId);
    Task<ScheduledWorkout?> GetByIdWithWorkout(Guid scheduledWorkoutId, Guid userId);
    Task<ScheduledWorkout?> GetByIdWithWorkoutAndExerciseProgresses(Guid scheduledWorkoutId, Guid userId);
    Task<ScheduledWorkout?> GetByIdAsync(Guid scheduledWorkoutId, Guid userId);
    void Delete(ScheduledWorkout scheduledWorkout);
    Task SaveChangesAsync();
}
