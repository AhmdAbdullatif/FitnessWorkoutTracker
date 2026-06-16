using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;

namespace Application.Features.ExerciseProgresses.AddNote;

public class AddNoteToExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor) : IAddNoteToExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId, AddNoteRequest req)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdWithNotesSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
            throw new NotFoundException($"Exercise progress with ID `{exerciseProgressId}` not found.");

        exerciseProgress.AddNote(req.Content);
        await repository.SaveChangesAsync();
    }
}
