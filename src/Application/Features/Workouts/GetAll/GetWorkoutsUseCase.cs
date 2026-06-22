using Application.Abstraction;
using Application.Specifications.Workouts;
using Ardalis.Specification;
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
        public async Task<GetWorkoutsResponse> ExecuteAsync(GetWorkoutsQuery query, string userZone)
        {
            if (string.IsNullOrWhiteSpace(userZone))
            {
                logger.LogDebug("Timezone information missing for retrieving workouts.");
                throw new DateTimeZoneNotFoundException("");
            }

            var userId = currentUserAccessor.GetId();

            var spec = new GetAllWorkoutsReadonlySpec(
                userId,
                query.SearchTerm?.Trim().ToLower(),
                sortOrder: query.SortOrder?.Trim().ToLower(),
                skip: (query.Page - 1) * query.PageSize,
                take: query.PageSize
            );

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
