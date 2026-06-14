using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstraction
{
    public interface IWorkoutRepository
    {
        Task<IEnumerable<Workout>> GetAllAsync(Guid userId);
        Task<Workout?> GetByIdWithExercisesAsync(Guid workoutId, Guid userId);
        Task<Workout?> GetByIdWithScheduledWorkoutsAsync(Guid workoutId, Guid userId);
        Task<Workout?> GetByIdWithOrderedScheduledWorkoutsReadOnlyAsync(Guid workoutId, Guid userId);
        Task AddAsync(Workout workout);
        Task SaveChangesAsync();
    }
}
