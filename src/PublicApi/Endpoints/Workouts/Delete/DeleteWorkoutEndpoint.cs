using Application.Features.Workouts.Delete;
using FastEndpoints;
namespace PublicApi.Endpoints.Workouts.Delete;

public class DeleteWorkoutEndpoint(IDeleteWorkoutUseCase deleteWorkoutUseCase)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("api/workouts/{id}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var workoutId = Route<Guid>("id");

        await deleteWorkoutUseCase.ExecuteAsync(workoutId);

        await SendNoContentAsync(cancellation: ct);
    }
}
