using NodaTime;
using NodaTime.Extensions;

namespace Domain.Entities;

public class Workout
{
    private readonly List<Exercise> _exercises = [];
    private readonly List<ScheduledWorkout> _scheduledWorkouts = [];
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
    public IReadOnlyCollection<Exercise> Exercises => _exercises;
    public IReadOnlyCollection<ScheduledWorkout> ScheduledWorkouts => _scheduledWorkouts;

    public void AddExercise(Exercise exercise)
    {
        ArgumentNullException.ThrowIfNull(exercise, nameof(exercise));
        _exercises.Add(exercise);
        ExercisesCount++;
    }
    public bool HasExercises => ExercisesCount > 0;

    public void AddScheduledWorkout(ScheduledWorkout scheduledWorkout)
    {
        ArgumentNullException.ThrowIfNull(scheduledWorkout, nameof(scheduledWorkout));
        _scheduledWorkouts.Add(scheduledWorkout);
    }

    public void UpdateDetails(string title, string? description)
    {
        ArgumentNullException.ThrowIfNull(title);

        Title = title.Trim();
        Description = description?.Trim();
    }
}
