using Application.Features.ScheduledWorkouts.Cancel;
using Application.Features.ScheduledWorkouts.GetAll;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ScheduledWorkouts.Cancel;

public class CancelScheduledWorkoutEndpoint(ICancelScheduledWorkoutUseCase cancelScheduledWorkoutUseCase)
    : EndpointWithoutRequest<ScheduledWorkoutDto>
{
    public override void Configure()
    {
        Put("api/workouts/scheduled-workouts/{scheduledWorkoutId}/Cancel");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var scheduledWorkoutId = Route<Guid>("scheduledWorkoutId");

        var scheduledWorkoutDto = await cancelScheduledWorkoutUseCase.ExecuteAsync(scheduledWorkoutId, userZone);

        await SendAsync(scheduledWorkoutDto, cancellation: ct);
    }

}
