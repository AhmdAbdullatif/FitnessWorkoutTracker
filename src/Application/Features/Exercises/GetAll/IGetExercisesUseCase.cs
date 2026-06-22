namespace Application.Features.Exercises.GetAll
{
    public interface IGetExercisesUseCase
    {
        Task<GetExercisesResponse> ExecuteAsync(GetExercisesQuery req, Guid workoutId, string userZone);
    }
}
