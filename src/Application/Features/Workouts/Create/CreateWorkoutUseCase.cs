using Application.Abstraction;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application.Features.Workouts.Create
{
    public class CreateWorkoutUseCase(IRepository<Workout> repository,
        ICurrentUserAccessor currentUserAccessor
    ) : ICreateWorkoutUseCase
    {
        public async Task<CreateWorkoutResponse> ExecuteAsync(CreateWorkoutCommand command)
        {
            var userId = currentUserAccessor.GetId();

            var workout = new Workout(command.Title, command.Description, userId);

            await repository.AddAsync(workout);

            return new CreateWorkoutResponse()
            {
                WorkoutId = workout.Id,
                Title = command.Title,
                Description = command.Description
            };
        }
    }
}
