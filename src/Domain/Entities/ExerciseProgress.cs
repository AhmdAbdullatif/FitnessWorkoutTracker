using Domain.Exceptions;
using NodaTime;

namespace Domain.Entities;

public class ExerciseProgress
{
    private ExerciseProgress() { } // required by EF Core
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
    public Instant? StartedAt { get; private set; }
    public Instant? CompletedAt { get; private set; }
    public ICollection<Note> Notes { get; private set; } = [];
    public Guid ScheduledWorkoutId { get; private set; }
    public ScheduledWorkout? ScheduledWorkout { get; private set; }
    public Guid ExerciseId { get; private set; }
    public Exercise? Exercise { get; private set; }

    public void Start(int sets, int reps)
    {
        ArgumentNullException.ThrowIfNull(ScheduledWorkout, nameof(ScheduledWorkout));

        if (ScheduledWorkout.Status != WorkoutStatus.InProgress)
            throw new ScheduledWorkoutNotInProgressException(ScheduledWorkout.Id);

        if (Status != ExerciseStatus.Pending)
            throw new ExerciseNotPendingException(Id);

        if (sets <= 0)
            throw new NegativeNumberException("Sets can't be zero or negative.");

        if (reps <= 0)
            throw new NegativeNumberException("Reps can't be zero or negative.");

        Status = ExerciseStatus.InProgress;
        StartedAt = SystemClock.Instance.GetCurrentInstant();

        Sets = sets;
        Reps = reps;
    }

    public void UpdateStatus(ExerciseStatus status)
    {
        ArgumentNullException.ThrowIfNull(ScheduledWorkout, nameof(ScheduledWorkout));

        if (ScheduledWorkout.Status != WorkoutStatus.InProgress)
            throw new ScheduledWorkoutNotInProgressException(ScheduledWorkout.Id);

        if (this.Status == ExerciseStatus.Pending)
            throw new ExerciseCannotBeUpdatedException(Id);
        
        if (status == ExerciseStatus.Completed && this.Status != ExerciseStatus.InProgress)
            throw new ExerciseNotInProgressException("Cannot complete an exercise that is not in progress.");

        if (this.Status == ExerciseStatus.Completed)
            CompletedAt = null;

        if (status == ExerciseStatus.Completed)
            CompletedAt = SystemClock.Instance.GetCurrentInstant();
        Status = status;
    }
    public void AddNote(string note)
    {
        ArgumentNullException.ThrowIfNull(note);
        Notes.Add(new Note(note, Id));
    }
}
