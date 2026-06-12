using Domain.Entities;

namespace Application.Features.ExerciseProgresses.Update;

public class UpdateExerciseProgressStatusRequest
{
    public ExerciseStatus Status { get; set; }
}
