using Domain.Entities;

namespace Application.Abstraction;

public interface IExerciseProgressRepository
{
    Task<ExerciseProgress?> GetByIdWithScheduledWorkout(Guid exerciseProgressId, Guid userId);
    Task SaveChangesAsync();
}
