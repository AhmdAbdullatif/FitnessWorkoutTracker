using Application.Features.Exercises.Get;

namespace PublicApi.Endpoints.Exercises.Get
{
    public class GetExercisesResponse
    {
        public List<ExerciseDto> ExerciseDtos { get; set; } = [];
    }
}
