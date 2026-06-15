using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Repositories
{
    public class WorkoutRepository(AppDbContext context) : IWorkoutRepository
    {
        public async Task<Workout?> GetByIdWithExercisesAsync(Guid workoutId, Guid userId)
        {
            return await context.Workouts
                .Include(x => x.Exercises)
                .FirstOrDefaultAsync(x => x.Id == workoutId && x.UserId == userId);
        }

       public async Task<IEnumerable<Workout>> GetAllAsync(Guid userId)
        {
            return await context.Workouts
            .AsNoTracking()
            .Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(Workout workout)
        {
            await context.Workouts.AddAsync(workout);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<Workout?> GetByIdWithScheduledWorkoutsAsync(Guid workoutId, Guid userId)
        {
            return await context.Workouts
                .Include(x => x.ScheduledWorkouts)
                .FirstOrDefaultAsync(x => x.Id == workoutId && x.UserId == userId);
        }

        public async Task<Workout?> GetByIdWithOrderedScheduledWorkoutsReadOnlyAsync(Guid workoutId, Guid userId)
        {
            return await context.Workouts
                .AsNoTracking()
                .Include(x => x.ScheduledWorkouts.OrderBy(sw => sw.SessionDate))
                .FirstOrDefaultAsync(x => x.Id == workoutId && x.UserId == userId);
        }

        public async Task<Workout?> GetByIdAsync(Guid workoutId, Guid userId)
        {
            return await context.Workouts
                .FirstOrDefaultAsync(x => x.Id == workoutId && x.UserId == userId);
        }

        public void Delete(Workout workout)
        {
            context.Workouts.Remove(workout);
        }
    }
}
