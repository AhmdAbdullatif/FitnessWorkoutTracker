using Application.Features.ExerciseProgresses.Update;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ExerciseProgresses.Update;

public class UpdateExerciseProgressStatusEndpoint(
    UpdateExerciseProgressStatusUseCase updateExerciseProgressStatusUseCase)
        : Endpoint<UpdateExerciseProgressStatusRequest, UpdateExerciseProgressStatusResponse>
{
    public override void Configure()
    {
        Post("api/exercises/update-status/{exerciseProgressId}");
    }

    public override async Task HandleAsync(UpdateExerciseProgressStatusRequest req, CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var exerciseProgressId = Route<Guid>("exerciseProgressId");

        var response = await updateExerciseProgressStatusUseCase.ExecuteAsync(exerciseProgressId, userZone, req);

        await SendAsync(response, cancellation: ct);
    }

}
