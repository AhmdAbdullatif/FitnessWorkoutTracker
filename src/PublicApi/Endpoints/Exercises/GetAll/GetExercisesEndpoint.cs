using Application.Features.Exercises.GetAll;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.Exercises.GetAll
{
    public class GetExercisesEndpoint(IGetExercisesUseCase getExercisesUseCases)
        : Endpoint<GetExercisesEndpointRequest, GetExercisesResponse>
    {
        public override void Configure()
        {
            Get("api/workouts/{workoutId}/exercises");
        }

        public override async Task HandleAsync(GetExercisesEndpointRequest req, CancellationToken ct)
        {
            var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

            var workoutId = Route<Guid>("workoutId");

            var query = new GetExercisesQuery(req.Page, req.PageSize, req.SearchTerm, req.SortOrder);

            var response = await getExercisesUseCases.ExecuteAsync(query, workoutId, userZone);

            await SendAsync(response, cancellation: ct);
        }
    }
}
