using Application.Abstraction;
using Application.Exceptions;
using Application.Features.Workouts.Create;
using Application.Specifications.Workouts;
using Domain.Entities;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Exercises.GetAll
{
    public class GetExercisesUseCases(IRepository<Workout> repository,
        ICurrentUserAccessor currentUserAccessor,
        IUtcLocalConverter utcLocalConverter) : IGetExercisesUseCases
    {
        public async Task<GetExercisesResponse> ExecuteAsync(Guid workoutId, string userZone)
        {
            if (string.IsNullOrWhiteSpace(userZone))
                throw new DateTimeZoneNotFoundException("");
                
            var userId = currentUserAccessor.GetId();

            var spec = new GetWorkoutByIdWithExercisesSpec(workoutId, userId);
            Workout? workout = await repository.FirstOrDefaultAsync(spec);

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
                ExerciseDtos = exerciseDtos
            };
        }
    }
}
