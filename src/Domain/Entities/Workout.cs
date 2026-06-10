using NodaTime;
using NodaTime.Extensions;

namespace Domain.Entities;

public class Workout
{
    private Workout() { } // required by EF Core
    public Workout(string title, string? description, Guid userId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
        UserId = userId;
        ExercisesCount = 0;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int ExercisesCount { get; private set; }
    public Instant CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public ICollection<Exercise> Exercises { get; private set; } = [];
    public ICollection<ScheduledWorkout> ScheduledWorkouts { get; private set; } = [];

    public void AddExercise(Exercise exercise)
    {
        Exercises.Add(exercise);
        ExercisesCount++;
    }
    public bool HasExercises => ExercisesCount > 0;

    public void AddScheduledWorkout(ScheduledWorkout scheduledWorkout)
    {
        ScheduledWorkouts.Add(scheduledWorkout);
    }
}
