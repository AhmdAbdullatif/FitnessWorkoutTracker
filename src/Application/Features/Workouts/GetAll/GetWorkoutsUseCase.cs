using Application.Abstraction;
using Application.Specifications;
using Application.Specifications.Workouts;
using Domain.Entities;
using NodaTime.TimeZones;

namespace Application.Features.Workouts.GetAll
{
    public class GetWorkoutsUseCase(IReadRepository<Workout> readRepository,
        ICurrentUserAccessor currentUserAccessor,
        IUtcLocalConverter utcLocalConverter,
        IAppLogger<GetWorkoutsUseCase> logger
    ) : IGetWorkoutsUseCase
    {
        public async Task<GetWorkoutsResponse> ExecuteAsync(string userZone)
        {
            if (string.IsNullOrWhiteSpace(userZone))
            {
                logger.LogDebug("Timezone information missing for retrieving exercise progresses.");
                throw new DateTimeZoneNotFoundException("");
            }

            var userId = currentUserAccessor.GetId();

            var spec = new GetAllWorkoutsReadonlySpec(userId);

            var workouts = await readRepository.ListAsync(spec);

            var workoutDtos = workouts.Select(x => new WorkoutDto()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ExercisesCount = x.ExercisesCount,
                CreatedAt = utcLocalConverter.ConvertUtcToLocal(x.CreatedAt, userZone)
            });

            return new GetWorkoutsResponse()
            {
                Workouts = workoutDtos
            };
        }
    }
}
