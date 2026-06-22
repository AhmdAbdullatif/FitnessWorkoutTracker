using FastEndpoints;

namespace PublicApi.Endpoints.ScheduledWorkouts.GetAll;

public class GetScheduledWorkoutsEndpointRequest
{
    [QueryParam]
    public string? SortOrder { get; set; }
    [QueryParam]
    public int Page { get; set; }
    [QueryParam]
    public int PageSize { get; set; }
}