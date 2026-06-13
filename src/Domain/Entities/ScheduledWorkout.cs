using System.Net;
using Domain.Exceptions;
using NodaTime;

namespace Domain.Entities;

public class ScheduledWorkout
{
    private ScheduledWorkout() { }

    public Guid Id { get; private set; }
    public Instant SessionDate { get; private set; }
    public Instant? StartedAt { get; private set; }
    public Instant? CompletedAt { get; private set; }
    public WorkoutStatus Status { get; private set; }
    public Guid WorkoutId { get; private set; }
    public Workout? Workout { get; private set; }
    public ICollection<ExerciseProgress> ExerciseProgresses { get; private set; } = [];

    public static ScheduledWorkout Schedule(Workout workout, Instant sessionDate)
    {
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
        if (Status != WorkoutStatus.Pending)
            throw new ScheduledWorkoutNotPendingException(Id);

        ArgumentNullException.ThrowIfNull(Workout, nameof(Workout));

        Status = WorkoutStatus.InProgress;
        StartedAt = SystemClock.Instance.GetCurrentInstant();

        foreach (var exercise in Workout.Exercises)
        {
            ExerciseProgresses.Add(new ExerciseProgress(exercise.Id, this));
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
        if (Status != WorkoutStatus.InProgress)
            throw new ScheduledWorkoutNotInProgressException(Id);

        foreach (var exerciseProgress in ExerciseProgresses)
        {
            if (exerciseProgress.Status != ExerciseStatus.Completed)
                exerciseProgress.Skip();
        }

        Status = WorkoutStatus.Canceled;
    }
}
