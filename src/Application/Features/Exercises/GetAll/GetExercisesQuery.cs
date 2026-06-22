namespace Application.Features.Exercises.GetAll;

public record GetExercisesQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortOrder = null);

