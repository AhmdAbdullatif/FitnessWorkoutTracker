using Application.Abstraction;
using Application.Exceptions;
using Application.Features.ExerciseProgresses.GetAll;
using Application.Features.Workouts.Create;
using NodaTime.TimeZones;

namespace Application.Features.ExerciseProgresses.GetById;

public class GetExerciseProgressByIdUseCase(IExerciseProgressRepository exerciseProgressRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter)
{
    public async Task<GetExerciseProgressByIdResponse> ExecuteAsync(Guid exerciseProgressId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");

        var userId = currentUserAccessor.GetId();

        var exerciseProgress = await exerciseProgressRepository
            .GetByIdWithExerciseAndNotes(exerciseProgressId, userId);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress with ID `{exerciseProgressId}` not found.");

        var response = new GetExerciseProgressByIdResponse()
        {
            Id = exerciseProgress.Id,
            Title = exerciseProgress.Exercise!.Title,
            Description = exerciseProgress.Exercise!.Description,
            Sets = exerciseProgress.Sets,
            Reps = exerciseProgress.Reps,
            Status = exerciseProgress.Status,
            StartedAt = utcLocalConverter.ConvertUtcToLocal(exerciseProgress.StartedAt.GetValueOrDefault(), userZone),
            CompletedAt = utcLocalConverter.ConvertUtcToLocal(exerciseProgress.CompletedAt.GetValueOrDefault(), userZone),
            Notes = exerciseProgress.Notes.Select(x => new NoteDto()
            {
                Id = x.Id,
                Content = x.Content
            })
        };

        return response;
    }
}
