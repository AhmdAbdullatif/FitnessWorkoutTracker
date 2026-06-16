using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.ExerciseProgresses.GetById;

public class GetExerciseProgressByIdUseCase(IReadRepository<ExerciseProgress> readRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUtcLocalConverter utcLocalConverter) : IGetExerciseProgressByIdUseCase
{
    public async Task<GetExerciseProgressByIdResponse> ExecuteAsync(Guid exerciseProgressId, string userZone)
    {
        if (string.IsNullOrWhiteSpace(userZone))
            throw new DateTimeZoneNotFoundException("");

        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdWithExerciseAndNotesReadonlySpec(exerciseProgressId, userId);

        var exerciseProgress = await readRepository.FirstOrDefaultAsync(spec);

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
            Notes = exerciseProgress.Notes.Select(x => new NoteDto()
            {
                Id = x.Id,
                Content = x.Content
            })

        };
        if (exerciseProgress.StartedAt is not null)
            response.StartedAt = utcLocalConverter
                .ConvertUtcToLocal(exerciseProgress.StartedAt.GetValueOrDefault(), userZone);

        if (exerciseProgress.CompletedAt is not null)
            response.CompletedAt = utcLocalConverter
                .ConvertUtcToLocal(exerciseProgress.CompletedAt.GetValueOrDefault(), userZone);


        return response;
    }
}
