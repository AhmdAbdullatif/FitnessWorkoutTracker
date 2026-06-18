using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications.ExerciseProgresses;
using Domain.Entities;

namespace Application.Features.ExerciseProgresses.AddNote;

public class AddNoteToExerciseProgressUseCase(IRepository<ExerciseProgress> repository,
    ICurrentUserAccessor currentUserAccessor,
    IAppLogger<AddNoteToExerciseProgressUseCase> logger) : IAddNoteToExerciseProgressUseCase
{
    public async Task ExecuteAsync(Guid exerciseProgressId, AddNoteRequest req)
    {
        var userId = currentUserAccessor.GetId();

        var spec = new GetExerciseProgressByIdWithNotesSpec(exerciseProgressId, userId);

        var exerciseProgress = await repository.FirstOrDefaultAsync(spec);

        if (exerciseProgress is null)
        {
            logger.LogInformation("Exercise progress with ID `{ExerciseProgressId}` not found to add a note.", 
                exerciseProgressId);
            
            throw new NotFoundException($"Exercise progress with ID `{exerciseProgressId}` not found.");
        }

        var noteId = exerciseProgress.AddNote(req.Content);

        logger.LogInformation("Note created.\nNoteId: {NoteId}\nExerciseProgressId: {ExerciseProgressId}\nUserId: {UserId}",
            noteId,
            exerciseProgress.Id,
            userId);
            
        await repository.SaveChangesAsync();
    }
}
