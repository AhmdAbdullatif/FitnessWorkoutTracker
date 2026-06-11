using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class ExerciseProgressRepository(AppDbContext context) : IExerciseProgressRepository
{
    public async Task<ExerciseProgress?> GetByIdWithScheduledWorkout(Guid exerciseProgressId, Guid userId)
    {
        return await context.ExerciseProgresses
            .Include(x => x.ScheduledWorkout)
            .FirstOrDefaultAsync(x => x.Id == exerciseProgressId &&
                x.ScheduledWorkout!.Workout!.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
