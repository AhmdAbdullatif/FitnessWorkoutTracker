using NodaTime;

namespace Domain.Entities;

public class Exercise
{
    public Exercise(string title, string? description, Guid workoutId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        WorkoutId = workoutId; 
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Instant CreatedAt { get; private set; }
    public Guid WorkoutId { get; private set; }
    public Workout? Workout { get; private set; }
    public ICollection<ExerciseProgress> ExerciseProgresses { get; private set; } = [];   
}
