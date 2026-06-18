using Application.Features.Exercises.GetAll;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.Exercises.GetAll
{
    public class GetExercisesEndpoint(IGetExercisesUseCase getExercisesUseCases)
        : EndpointWithoutRequest<GetExercisesResponse>
    {
        public override void Configure()
        {
            Get("api/workouts/{workoutId}/exercises");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

            var workoutId = Route<Guid>("workoutId");

            var response = await getExercisesUseCases.ExecuteAsync(workoutId, userZone);

            await SendAsync(response, cancellation: ct);
        }
    }
}
