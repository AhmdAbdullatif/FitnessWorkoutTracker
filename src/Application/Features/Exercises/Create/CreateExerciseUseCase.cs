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
        ICurrentUserAccessor currentUserAccessor,
        IAppLogger<CreateExerciseUseCase> logger
    ) : ICreateExerciseUseCase
    {
        public async Task<CreateExerciseResponse> ExecuteAsync(Guid workoutId, CreateExerciseRequest req)
        {
            var userId = currentUserAccessor.GetId();

            var spec = new GetWorkoutByIdWithExercisesSpec(workoutId, userId);

            Workout? workout = await repository.FirstOrDefaultAsync(spec);

            if (workout is null)
            {
                logger.LogInformation("Workout not found for creating an exercise.\nWorkoutId: {WorkoutId}.",
                    workoutId);

                throw new NotFoundException($"Workout with ID: {workoutId} not found.");
            }

            var exercise = new Exercise(req.Title, req.Description, workoutId);
            workout.AddExercise(exercise);

            await repository.SaveChangesAsync();

            logger.LogInformation("Exercise created.\nExerciseId: {ExerciseId}\nUserId: {UserId}",
                exercise.Id,
                userId);

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
