namespace Application.Features.Workouts.GetById;

public class GetWorkoutByIdResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ExercisesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<ScheduledWorkoutDto> ScheduledWorkoutDtos { get; set; } = [];

}
