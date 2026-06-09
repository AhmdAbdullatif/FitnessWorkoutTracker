using Application.Features.Exercises.Get;
using FastEndpoints;

namespace PublicApi.Endpoints.Exercises.Get
{
    public class GetExercisesEndpoint(GetExercisesUseCases getExercisesUseCases, 
        AutoMapper.IMapper mapper) : EndpointWithoutRequest<GetExercisesResponse>
    {
        public override void Configure()
        {
            Get("api/workouts/{workoutId}/exercises");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var workoutId = Route<Guid>("workoutId");

            var exercises = await getExercisesUseCases.Execute(workoutId);

            var exerciseDtos = exercises.Select(x => mapper.Map<ExerciseDto>(x));

            await SendAsync(new GetExercisesResponse()
            {
                ExerciseDtos = [.. exerciseDtos]
            });
        }
    }
}
