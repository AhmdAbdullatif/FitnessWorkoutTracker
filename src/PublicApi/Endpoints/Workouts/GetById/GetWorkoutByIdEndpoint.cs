using Application.Features.Workouts.GetById;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.Workouts.GetById;

public class GetWorkoutByIdEndpoint(IGetWorkoutByIdUseCase getWorkoutByIdUseCase)
    : EndpointWithoutRequest<GetWorkoutByIdResponse>
{
    public override void Configure()
    {
        Get("api/workouts/{id}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var workoutId = Route<Guid>("id");

        var response = await getWorkoutByIdUseCase.ExecuteAsync(workoutId, userZone);

        await SendAsync(response, cancellation: ct);
    }
}
