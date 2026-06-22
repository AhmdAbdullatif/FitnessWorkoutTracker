namespace Application.Features.Workouts.GetAll
{
    public interface IGetWorkoutsUseCase
    {
        Task<GetWorkoutsResponse> ExecuteAsync(GetWorkoutsQuery query, string userZone);
    }
}
