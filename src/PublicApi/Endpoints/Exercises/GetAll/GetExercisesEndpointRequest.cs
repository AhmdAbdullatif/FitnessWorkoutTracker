using FastEndpoints;

namespace PublicApi.Endpoints.Exercises.GetAll
{
    public class GetExercisesEndpointRequest
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
}
