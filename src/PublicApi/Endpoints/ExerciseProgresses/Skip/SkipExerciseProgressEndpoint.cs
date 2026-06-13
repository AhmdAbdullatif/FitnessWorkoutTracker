using Application.Features.ExerciseProgresses.Skip;
using FastEndpoints;

namespace PublicApi.Endpoints.ExerciseProgresses.Skip;

public class SkipExerciseProgressEndpoint(ISkipExerciseProgressUseCase skipExerciseProgressUseCase)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("api/exercise-progresses/{id}/skip");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var exerciseProgressId = Route<Guid>("id");

        await skipExerciseProgressUseCase.ExecuteAsync(exerciseProgressId);

        await SendNoContentAsync(cancellation: ct);
    }

}
