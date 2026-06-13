using Application.Features.ExerciseProgresses.Start;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ExerciseProgresses.Start;

public class StartExerciseProgressEndpoint(IStartExerciseProgressUseCase startExerciseProgressUseCase) : Endpoint<StartExerciseRequest, StartExerciseResponse>
{
    public override void Configure()
    {
        Post("api/exercise-progresses/{id}/start");
    }

    public override async Task HandleAsync(StartExerciseRequest req, CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();
        
        var exerciseProgressId = Route<Guid>("id");

        var response = await startExerciseProgressUseCase.ExecuteAsync(exerciseProgressId, req, userZone);

        await SendAsync(response, cancellation: ct);
    }

}
