namespace Application.Features.ExerciseProgresses.Skip;

public interface ISkipExerciseProgressUseCase
{
    Task ExecuteAsync(Guid exerciseProgressId);
}