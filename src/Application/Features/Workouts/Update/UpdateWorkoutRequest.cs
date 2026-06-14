namespace Application.Features.Workouts.Update;

public class UpdateWorkoutRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description{ get; set; }
}
