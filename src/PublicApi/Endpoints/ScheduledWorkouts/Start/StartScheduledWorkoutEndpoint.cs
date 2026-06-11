using Application.Features.ScheduledWorkouts.Start;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ScheduledWorkouts.Start;

public class StartScheduledWorkoutEndpoint(StartScheduledWorkoutUseCase startScheduledWorkoutUseCase)
    : EndpointWithoutRequest<StartScheduledWorkoutResponse>
{
    public override void Configure()
    {
        Post("api/workouts/start/{scheduledWorkoutId}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduledWorkoutId = Route<Guid>("scheduledWorkoutId");

        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var response = await startScheduledWorkoutUseCase.ExecuteAsync(scheduledWorkoutId, userZone);

        await SendAsync(response, cancellation: ct);
    }

}
