namespace Application.Features.Exercises.GetAll
{
    public interface IGetExercisesUseCase
    {
        Task<GetExercisesResponse> ExecuteAsync(Guid workoutId, string userZone);
    }
}
