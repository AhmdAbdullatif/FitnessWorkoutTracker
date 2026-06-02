using NodaTime;
using NodaTime.Extensions;

namespace Domain.Entities;

public class Workout
{
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
    public ICollection<Exercise> Exercises { get; private set; } = [];
    public ICollection<ScheduledWorkout> ScheduledWorkouts { get; private set; } = [];
}
