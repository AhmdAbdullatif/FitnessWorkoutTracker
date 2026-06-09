using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Exercises.Get
{
    public class GetExercisesUseCases(IWorkoutRepository workoutRepository, 
        ICurrentUserAccessor currentUserAccessor)
    {
        public async Task<IEnumerable<Exercise>> Execute(Guid workoutId)
        {
            var userId = currentUserAccessor.GetId();

            var workout = await workoutRepository.GetByIdWithExercisesAsync(workoutId, userId);
            if (workout is null)
                throw new NotFoundException($"Workout with ID `{workoutId} not found.`");

            return workout.Exercises;
        }
    }
}
