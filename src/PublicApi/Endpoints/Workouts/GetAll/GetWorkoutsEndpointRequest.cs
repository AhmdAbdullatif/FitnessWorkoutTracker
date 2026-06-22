using FastEndpoints;

namespace PublicApi.Endpoints.Workouts.GetAll;

public class GetWorkoutsEndpointRequest
{
    [QueryParam]
    public string? SearchTerm { get; set; }
    [QueryParam]
    public string? SortOrder { get; set; }
    [QueryParam]
    public int Page { get; set; }
    [QueryParam]
    public int PageSize { get; set; }

}