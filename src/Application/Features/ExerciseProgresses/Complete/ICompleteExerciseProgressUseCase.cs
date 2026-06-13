namespace Application.Features.ExerciseProgresses.Complete;

public interface ICompleteExerciseProgressUseCase
{
    Task ExecuteAsync(Guid exerciseProgressId);
}