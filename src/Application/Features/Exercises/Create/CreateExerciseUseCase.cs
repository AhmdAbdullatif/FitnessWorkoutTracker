using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Application.Specifications.Workouts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Exercises.Create
{
    public class CreateExerciseUseCase(IRepository<Workout> repository,
        ICurrentUserAccessor currentUserAccessor
    ) : ICreateExerciseUseCase
    {
        public async Task<CreateExerciseResponse> ExecuteAsync(Guid workoutId, CreateExerciseRequest req)
        {
            var userId = currentUserAccessor.GetId();

            var spec = new GetWorkoutByIdWithExercisesSpec(workoutId, userId);

            Workout? workout = await repository.FirstOrDefaultAsync(spec);

            if (workout is null)
                throw new NotFoundException("Workout not found.");

            var exercise = new Exercise(req.Title, req.Description, workoutId);
            workout.AddExercise(exercise);

            await repository.SaveChangesAsync();
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
