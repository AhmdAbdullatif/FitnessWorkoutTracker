using Application.Abstraction;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application.Features.Workouts.Create
{
    public class CreateWorkoutUseCase(IWorkoutRepository workoutRepository,
        ICurrentUserAccessor currentUserAccessor
    )
    {
        public async Task<CreateWorkoutResponse> ExecuteAsync(CreateWorkoutCommand command)
        {
            var userId = currentUserAccessor.GetId();

            var workout = new Workout(command.Title, command.Description, userId);
            
            await workoutRepository.AddAsync(workout);
            await workoutRepository.SaveChangesAsync();

            return new CreateWorkoutResponse()
            {
                WorkoutId = workout.Id,
                Title = command.Title,
                Description = command.Description
            };
        }
    }
}
