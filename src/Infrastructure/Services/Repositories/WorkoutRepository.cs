using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Repository
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
            return await context.Workouts.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(Workout workout)
        {
            await context.Workouts.AddAsync(workout);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
