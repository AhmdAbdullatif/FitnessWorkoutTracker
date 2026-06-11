using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class ScheduledWorkoutRepository(AppDbContext context) : IScheduledWorkoutRepository
{
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

}
