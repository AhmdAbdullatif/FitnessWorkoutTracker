using Application.Features.ScheduledWorkouts.Delete;
using FastEndpoints;

namespace PublicApi.Endpoints.ScheduledWorkouts.Delete;

public class DeleteScheduledWorkoutEndpoint(IDeleteScheduledWorkoutUseCase deleteScheduledWorkoutUseCase)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("api/scheduled-workouts/{id}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduledWorkoutId = Route<Guid>("id");

        await deleteScheduledWorkoutUseCase.ExecuteAsync(scheduledWorkoutId);

        await SendNoContentAsync(ct);
    }

}
