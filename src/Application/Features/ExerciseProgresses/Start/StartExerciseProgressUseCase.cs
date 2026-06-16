using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ExerciseProgresses.Start;

public class StartExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter) : IStartExerciseProgressUseCase
{
    public async Task<StartExerciseResponse> ExecuteAsync(Guid exerciseProgressId,
        StartExerciseRequest request,
        string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");
            
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdWithScheduledWorkoutSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress `{exerciseProgressId}` not found.");

        exerciseProgress.Start(request.Sets, request.Reps);

        var response = new StartExerciseResponse()
        {
            Id = exerciseProgressId,
            StartedAt = utcLocalConverter
                .ConvertUtcToLocal(exerciseProgress.StartedAt.GetValueOrDefault(), userZone),

            Sets = exerciseProgress.Sets,
            Reps = exerciseProgress.Reps,
            Status = exerciseProgress.Status
        };
        
        await repository.SaveChangesAsync();

        return response;
    }
}
