using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class ScheduledWorkoutRepository(AppDbContext context) : IScheduledWorkoutRepository
{
    public async Task<IEnumerable<ScheduledWorkout>> GetAllWithWorkout(Guid workoutId, Guid userId)
    {
        return await context.ScheduledWorkouts
            .AsNoTracking()
            .Include(x => x.Workout)
            .Where(x => x.WorkoutId == workoutId && x.Workout!.UserId == userId)
            .ToListAsync();
    }

    public async Task<ScheduledWorkout?> GetByIdWithWorkout(Guid scheduledWorkoutId, Guid userId)
    {
        return await context.ScheduledWorkouts
            .AsNoTracking()
            .Include(x => x.Workout)
            .FirstOrDefaultAsync(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }

    public async Task<ScheduledWorkout?> GetByIdWithExerciseProgressesThenWithExercise(Guid scheduledWorkoutId, Guid userId)
    {
        return await context.ScheduledWorkouts
            .AsNoTracking()
            .Include(x => x.ExerciseProgresses)
            .ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }

    public async Task<ScheduledWorkout?> GetByIdWithWorkoutThenExercises(Guid scheduledWorkoutId)
    {
        return await context.ScheduledWorkouts
            .Include(x => x.Workout)
            .ThenInclude(x => x!.Exercises)
            .FirstOrDefaultAsync(x => x.Id == scheduledWorkoutId);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<ScheduledWorkout?> GetByIdWithWorkoutAndExerciseProgresses(Guid scheduledWorkoutId, Guid userId)
    {
        return await context.ScheduledWorkouts
            .AsSplitQuery()
            .Include(x => x.Workout)
            .Include(x => x.ExerciseProgresses)
            .FirstOrDefaultAsync(x => x.Id == scheduledWorkoutId && x.Workout!.UserId == userId);
    }
}
