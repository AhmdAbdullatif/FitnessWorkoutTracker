using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Update;

public class UpdateExerciseProgressStatusResponse
{
    public Guid Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public ExerciseStatus Status { get; set; }
}
