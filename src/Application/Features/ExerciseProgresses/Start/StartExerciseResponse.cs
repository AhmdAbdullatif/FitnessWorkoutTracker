using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Start;

public class StartExerciseResponse
{
    public Guid Id { get; set; }
    public DateTime StartedAt { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public ExerciseStatus Status { get; set; }

}
