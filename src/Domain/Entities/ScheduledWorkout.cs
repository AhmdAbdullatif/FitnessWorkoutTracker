using System.Net;
using Domain.Exceptions;
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
}
