using Application.Features.ExerciseProgresses.GetAll;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ExerciseProgresses.GetAll;

public class GetExerciseProgressesEndpoint(IGetExerciseProgressesUseCase getExerciseProgressesUseCase) : EndpointWithoutRequest<GetExerciseProgressesResponse>
{
    public override void Configure()
    {
        Get("api/scheduled-workouts/{scheduledWorkoutId}/exercise-progresses");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var scheduledWorkoutId = Route<Guid>("scheduledWorkoutId");

        var response = await getExerciseProgressesUseCase
            .ExecuteAsync(scheduledWorkoutId, userZone);

        await SendAsync(response, cancellation: ct);
    }
}
