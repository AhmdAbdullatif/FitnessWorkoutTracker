using Application.Abstraction;
using Application.Features.Workouts.Create;
using Domain.Entities;

namespace Application.Features.Workouts.GetAll
{
    public class GetWorkoutsUseCase(IWorkoutRepository workoutRepository,
        ICurrentUserAccessor currentUserAccessor
    )  
    {
        public async Task<GetWorkoutsResult> ExecuteAsync()
        {
            var userId = currentUserAccessor.GetId();

            var workouts =  await workoutRepository.GetAllAsync(userId);

            return new GetWorkoutsResult()
            {
                Workouts = [.. workouts]
            };
        }
    }
}
