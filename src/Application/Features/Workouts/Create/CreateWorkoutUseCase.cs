using Application.Abstraction;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application.Features.Workouts.Create
{
    public class CreateWorkoutUseCase(IRepository<Workout> repository,
        ICurrentUserAccessor currentUserAccessor,
        IAppLogger<CreateWorkoutUseCase> logger
    ) : ICreateWorkoutUseCase
    {
        public async Task<CreateWorkoutResponse> ExecuteAsync(CreateWorkoutCommand command)
        {
            var userId = currentUserAccessor.GetId();

            var workout = new Workout(command.Title, command.Description, userId);

            await repository.AddAsync(workout);

            logger.LogInformation("Workout created\nWorkoutId: {WorkoutId}\nUserId: {UserId}",
                workout.Id,
                userId);

            return new CreateWorkoutResponse()
            {
                WorkoutId = workout.Id,
                Title = command.Title,
                Description = command.Description
            };
        }
    }
}
