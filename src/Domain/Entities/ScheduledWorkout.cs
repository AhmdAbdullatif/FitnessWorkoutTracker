using System.Net;
using NodaTime;

namespace Domain.Entities;

public class ScheduledWorkout
{
    private ScheduledWorkout() { }

    public Guid Id { get; private set; }
    public Instant SessionDate { get; private set; }
    public Instant StartedAt { get; private set; }
    public Instant CompletedAt { get; private set; }
    public WorkoutStatus Status { get; private set; }
    public Guid WorkoutId { get; private set; }
    public Workout? Workout { get; private set; }
    public ICollection<ExerciseProgress> ExerciseProgresses { get; private set; } = [];
}
