using System.Net;
using Domain.Exceptions;
using NodaTime;

namespace Domain.Entities;

public class ScheduledWorkout
{
    private readonly List<ExerciseProgress> _exerciseProgresses = [];
    private ScheduledWorkout() { }

    public Guid Id { get; private set; }
    public Instant SessionDate { get; private set; }
    public Instant? StartedAt { get; private set; }
    public Instant? CompletedAt { get; private set; }
    public WorkoutStatus Status { get; private set; }
    public Guid WorkoutId { get; private set; }
    public Workout? Workout { get; private set; }
    public IReadOnlyCollection<ExerciseProgress> ExerciseProgresses => _exerciseProgresses;

    public static ScheduledWorkout Schedule(Workout workout, Instant sessionDate)
    {
        ArgumentNullException.ThrowIfNull(workout, nameof(workout));

        if (!workout.HasExercises)
            throw new WorkoutWithoutExercisesException(workout.Id);

        if (sessionDate < SystemClock.Instance.GetCurrentInstant())
            throw new SessionDateNotInTheFutureException();

        return new ScheduledWorkout()
        {
            Id = Guid.NewGuid(),
            SessionDate = sessionDate,
            Status = WorkoutStatus.Pending,
            WorkoutId = workout.Id,
            Workout = workout
        };
    }

    public void Start()
    {
        ArgumentNullException.ThrowIfNull(Workout, nameof(Workout));
        
        if (Status != WorkoutStatus.Pending)
            throw new ScheduledWorkoutNotPendingException(Id);

        Status = WorkoutStatus.InProgress;
        StartedAt = SystemClock.Instance.GetCurrentInstant();

        foreach (var exercise in Workout.Exercises)
        {
            _exerciseProgresses.Add(new ExerciseProgress(exercise.Id, this));
        }
    }
    public void Finish()
    {
        if (Status != WorkoutStatus.InProgress)
            throw new ScheduledWorkoutNotInProgressException(Id);

        CompletedAt = SystemClock.Instance.GetCurrentInstant();

        foreach (var exerciseProgress in ExerciseProgresses)
        {
            if (exerciseProgress.Status != ExerciseStatus.Completed)
                exerciseProgress.Skip();
        }
        
        Status = WorkoutStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == WorkoutStatus.Completed)
            throw new ScheduledWorkoutCannotBeCanceledException("Cannot cancel a completed scheduled workout.");
        
        if (Status == WorkoutStatus.Canceled)
            return;

        foreach (var exerciseProgress in ExerciseProgresses)
        {
            if (exerciseProgress.Status != ExerciseStatus.Completed)
                exerciseProgress.Skip();
        }

        Status = WorkoutStatus.Canceled;
    }

    public void Reschedule(Instant sessionInstant)
    {
        if (Status != WorkoutStatus.Pending)
            throw new ScheduledWorkoutNotPendingException("Cannot reschedule a workout session that was started before.");

        if (sessionInstant < SystemClock.Instance.GetCurrentInstant())
            throw new SessionDateNotInTheFutureException();

        SessionDate = sessionInstant;
    }
}
