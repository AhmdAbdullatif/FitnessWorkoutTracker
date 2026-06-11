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
        ICurrentUserAccessor currentUserAccessor,
        IUtcLocalConverter utcLocalConverter)
    {
        public async Task<GetExercisesResponse> ExecuteAsync(Guid workoutId, string userZone)
        {
            var userId = currentUserAccessor.GetId();

            var workout = await workoutRepository.GetByIdWithExercisesAsync(workoutId, userId);
            if (workout is null)
                throw new NotFoundException($"Workout with ID `{workoutId} not found.`");


            var exerciseDtos = workout.Exercises.Select(x => new ExerciseDto()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                CreatedAt = utcLocalConverter.ConvertUtcToLocal(x.CreatedAt, userZone)
            });

            return new GetExercisesResponse()
            {
                ExerciseDtos = [.. exerciseDtos]
            };
        }
    }
}
