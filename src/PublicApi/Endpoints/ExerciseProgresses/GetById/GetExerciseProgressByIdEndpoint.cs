using Application.Features.ExerciseProgresses.GetById;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ExerciseProgresses.GetById;

public class GetExerciseProgressByIdEndpoint(GetExerciseProgressByIdUseCase getExerciseProgressByIdUseCase)
    : EndpointWithoutRequest<GetExerciseProgressByIdResponse>
{
    public override void Configure()
    {
        Get("api/workouts/schedule/exercises/{exerciseProgressId}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var exerciseProgressId = Route<Guid>("exerciseProgressId");
    
        var response =  await getExerciseProgressByIdUseCase.ExecuteAsync(exerciseProgressId, userZone);

        await SendAsync(response, cancellation: ct);
    }

}
