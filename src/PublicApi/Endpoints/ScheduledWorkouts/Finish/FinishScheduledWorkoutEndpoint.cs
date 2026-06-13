using Application.Features.ScheduledWorkouts.Finish;
using Application.Features.ScheduledWorkouts.GetAll;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ScheduledWorkouts.Finish;

public class FinishScheduledWorkoutEndpoint(IFinishScheduledWorkoutUseCase finishScheduledWorkoutUseCase)
    : EndpointWithoutRequest<ScheduledWorkoutDto>
{
    public override void Configure()
    {
        Post("api/scheduled-workouts/{id}/finish");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var scheduledWorkoutId = Route<Guid>("id");

        var scheduledWorkoutDto = await finishScheduledWorkoutUseCase.ExecuteAsync(scheduledWorkoutId, userZone);

        await SendAsync(scheduledWorkoutDto, cancellation: ct);
    }
}
