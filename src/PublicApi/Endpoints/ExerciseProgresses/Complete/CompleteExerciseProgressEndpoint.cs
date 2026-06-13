using Application.Features.ExerciseProgresses.Complete;
using FastEndpoints;

namespace PublicApi.Endpoints.ExerciseProgresses.Complete;

public class CompleteExerciseProgressEndpoint(ICompleteExerciseProgressUseCase completeExerciseProgressUseCase)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("api/exercise-progresses/{id}/complete");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var exerciseProgressId = Route<Guid>("id");

        await completeExerciseProgressUseCase.ExecuteAsync(exerciseProgressId);

        await SendNoContentAsync(cancellation: ct);
    }

}
