using Domain.Exceptions;
using NodaTime;

namespace Domain.Entities;

public class ExerciseProgress
{
    private readonly List<Note> _notes = [];

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
    public IReadOnlyCollection<Note> Notes => _notes;
    public Guid ScheduledWorkoutId { get; private set; }
    public ScheduledWorkout? ScheduledWorkout { get; private set; }
    public Guid ExerciseId { get; private set; }
    public Exercise? Exercise { get; private set; }

    public void Start(int sets, int reps)
    {
        ArgumentNullException.ThrowIfNull(ScheduledWorkout, nameof(ScheduledWorkout));

        if (ScheduledWorkout.Status != WorkoutStatus.InProgress)
            throw new ScheduledWorkoutNotInProgressException("Cannot start an exercise progress that is not in a running scheduled workout.");

        if (Status != ExerciseStatus.Pending)
            throw new ExerciseNotPendingException(Id);

        if (sets <= 0)
            throw new NegativeNumberException("Sets can't be zero or negative.");

        if (reps <= 0)
            throw new NegativeNumberException("Reps can't be zero or negative.");

        StartedAt = SystemClock.Instance.GetCurrentInstant();
        Sets = sets;
        Reps = reps;

        Status = ExerciseStatus.InProgress;
    }

    public void Complete()
    {
        ArgumentNullException.ThrowIfNull(ScheduledWorkout, nameof(ScheduledWorkout));

        if (ScheduledWorkout.Status != WorkoutStatus.InProgress)
            throw new ScheduledWorkoutNotInProgressException("Cannot complete an exercise progress that is not in a running scheduled workout.");

        if (Status != ExerciseStatus.InProgress)
            throw new ExerciseNotInProgressException("Cannot complete an exercise that is not currently in progress.");

        Status = ExerciseStatus.Completed;
        CompletedAt = SystemClock.Instance.GetCurrentInstant();
    }

    public void Skip()
    {
        ArgumentNullException.ThrowIfNull(ScheduledWorkout, nameof(ScheduledWorkout));

        if (ScheduledWorkout.Status == WorkoutStatus.Pending)
            throw new ScheduledWorkoutPendingException("Cannot skip an exercise progress while the scheduled workout not started yet.");

        if (Status == ExerciseStatus.Completed)
            throw new ExerciseProgressCannotBeCanceledException();

        Status = ExerciseStatus.Skipped;
    }
    public Guid AddNote(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var note = new Note(content, Id);
        _notes.Add(note);

        return note.Id;
    }
}
