using Application.Features.Workouts.GetAll;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.Workouts.GetAll
{
    public class GetWorkoutsEndpoint(IGetWorkoutsUseCase getWorkoutsUseCase) 
        : Endpoint<GetWorkoutsEndpointRequest, GetWorkoutsResponse>
    {
        public override void Configure()
        {
            Get("api/workouts");
        }

        public override async Task HandleAsync(GetWorkoutsEndpointRequest req, CancellationToken ct)
        {
            var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

            var query = new GetWorkoutsQuery(req.Page, req.PageSize, req.SearchTerm, req.SortOrder);

            var response = await getWorkoutsUseCase.ExecuteAsync(query, userZone);

            await SendAsync(response, cancellation: ct);
        }
    }
}
