using Application.Features.ExerciseProgresses.GetAll;
using Domain.Entities;

namespace Application.Abstraction;

public interface IExerciseProgressRepository
{
    Task<ExerciseProgress?> GetByIdWithScheduledWorkout(Guid exerciseProgressId, Guid userId);
    Task<ExerciseProgress?> GetByIdWithNotes(Guid exerciseProgressId, Guid userId);
    Task<ExerciseProgress?> GetByIdWithExerciseAndNotes(Guid exerciseProgressId, Guid userId);
    Task SaveChangesAsync();
}
