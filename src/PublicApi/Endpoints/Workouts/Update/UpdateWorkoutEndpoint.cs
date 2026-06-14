using Application.Features.Workouts.Update;
using FastEndpoints;

namespace PublicApi.Endpoints.Workouts.Update;

public class UpdateWorkoutEndpoint(IUpdateWorkoutUseCase updateWorkoutUseCase) 
    : Endpoint<UpdateWorkoutRequest>
{
    public override void Configure()
    {
        Put("api/workouts/{id}");
    }

    public override async Task HandleAsync(UpdateWorkoutRequest req, CancellationToken ct)
    {
        var workoutId = Route<Guid>("id");

        await updateWorkoutUseCase.ExecuteAsync(workoutId, req);

        await SendNoContentAsync(ct);
    }
}
