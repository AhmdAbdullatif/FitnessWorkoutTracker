using System.ComponentModel;
using System.Security;
using NodaTime;

namespace Domain.Entities;

public class ExerciseProgress
{
    public ExerciseProgress(Guid exerciseId, ScheduledWorkout scheduledWorkout)
    {
        Id = Guid.NewGuid();
        ExerciseId = exerciseId; 
        ScheduledWorkoutId = scheduledWorkout.Id;
        ScheduledWorkout = scheduledWorkout;
        Status = ExerciseStatus.Pending;
    }
    public Guid Id { get; private set; }
    public int Sets { get; private set; }
    public int Reps { get; private set; }
    public ExerciseStatus Status { get; private set; }
    public Instant StartedAt { get; private set; }
    public Instant CompletedAt { get; private set; }
    public ICollection<Note> Notes { get; private set; } = [];
    public Guid ScheduledWorkoutId { get; private set; }
    public ScheduledWorkout? ScheduledWorkout { get; private set; }
    public Guid ExerciseId { get; private set; }
    public Exercise? Exercise { get; private set; }
}
