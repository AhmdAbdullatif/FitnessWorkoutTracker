using Application.Features.ScheduledWorkouts.Start;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ScheduledWorkouts.Start;

public class StartScheduledWorkoutEndpoint(IStartScheduledWorkoutUseCase startScheduledWorkoutUseCase)
    : EndpointWithoutRequest<StartScheduledWorkoutResponse>
{
    public override void Configure()
    {
        Post("api/scheduled-workouts/{id}/start");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduledWorkoutId = Route<Guid>("id");

        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var response = await startScheduledWorkoutUseCase.ExecuteAsync(scheduledWorkoutId, userZone);

        await SendAsync(response, cancellation: ct);
    }

}
