using Application.Features.ScheduledWorkouts.GetAll;
using Application.Features.ScheduledWorkouts.GetById;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ScheduledWorkouts.GetById;

public class GetScheduledWorkoutByIdEndpoint(
    IGetScheduledWorkoutByIdUseCase getScheduledWorkoutByIdUseCase) 
    : EndpointWithoutRequest<ScheduledWorkoutDto>
{
    public override void Configure()
    {
        Get("api/scheduled-workouts/{id}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var scheduledWorkoutId = Route<Guid>("id");

        var scheduledWorkoutDto = await getScheduledWorkoutByIdUseCase.ExecuteAsync(scheduledWorkoutId, userZone);

        await SendAsync(scheduledWorkoutDto, cancellation: ct);
    }

}
