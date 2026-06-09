using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Exercises.Create
{
    public class CreateExerciseUseCase(IWorkoutRepository workoutRepository,
        ICurrentUserAccessor currentUserAccessor
    )
    {
        public async Task<CreateExerciseResponse> ExecuteAsync(Guid workoutId, CreateExerciseRequest req)
        {
            var userId = currentUserAccessor.GetId();

            Workout? workout = await workoutRepository.GetByIdWithExercisesAsync(workoutId, userId);
            if (workout is null)
                throw new NotFoundException("Workout not found.");

            var exercise = new Exercise(req.Title, req.Description, workoutId);
            workout.AddExercise(exercise);

            await workoutRepository.SaveChangesAsync();
            return new CreateExerciseResponse()
            {
                Id = exercise.Id,
                Title = exercise.Title,
                Description = exercise.Description,
                WorkoutId = workoutId
            };
        }
    }
}
