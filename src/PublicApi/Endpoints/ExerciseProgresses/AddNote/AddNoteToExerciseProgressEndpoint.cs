using Application.Features.ExerciseProgresses.AddNote;
using FastEndpoints;

namespace PublicApi.Endpoints.ExerciseProgresses.AddNote;

public class AddNoteToExerciseProgressEndpoint(IAddNoteToExerciseProgressUseCase addNoteToExerciseProgressUseCase)
    : Endpoint<AddNoteRequest>
{
    public override void Configure()
    {
        Post("api/exercise-progresses/{id}/notes");
    }

    public override async Task HandleAsync(AddNoteRequest req, CancellationToken ct)
    {
        var exerciseProgressId = Route<Guid>("id");

        await addNoteToExerciseProgressUseCase.ExecuteAsync(exerciseProgressId, req);

        await SendOkAsync(ct);
    }
}
