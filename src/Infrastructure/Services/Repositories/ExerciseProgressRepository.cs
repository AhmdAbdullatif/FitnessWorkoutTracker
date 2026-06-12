using Application.Abstraction;
using Application.Features.ExerciseProgresses.GetAll;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class ExerciseProgressRepository(AppDbContext context) : IExerciseProgressRepository
{
    public async Task<ExerciseProgress?> GetByIdWithNotes(Guid exerciseProgressId, Guid userId)
    {
        return await context.ExerciseProgresses
            .Include(x => x.Notes)
            .FirstOrDefaultAsync(x => x.Id == exerciseProgressId &&
                x.ScheduledWorkout!.Workout!.UserId == userId);
    }

    public async Task<ExerciseProgress?> GetByIdWithScheduledWorkout(Guid exerciseProgressId, Guid userId)
    {
        return await context.ExerciseProgresses
            .Include(x => x.ScheduledWorkout)
            .FirstOrDefaultAsync(x => x.Id == exerciseProgressId &&
                x.ScheduledWorkout!.Workout!.UserId == userId);
    }

    public async Task<ExerciseProgress?> GetByIdWithExerciseAndNotes(Guid exerciseProgressId, Guid userId)
    {
        return await context.ExerciseProgresses
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Exercise)
            .Include(x => x.Notes)
            .FirstOrDefaultAsync(x => x.Id == exerciseProgressId
            && x.ScheduledWorkout!.Workout!.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
