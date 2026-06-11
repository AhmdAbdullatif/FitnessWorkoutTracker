using Domain.Entities;

namespace Application.Features.ScheduledWorkouts.Start;

public class StartScheduledWorkoutResponse
{
    public Guid Id { get; set; }
    public DateTime StartedAt { get; set; }
    public WorkoutStatus Status { get; set; }
}
