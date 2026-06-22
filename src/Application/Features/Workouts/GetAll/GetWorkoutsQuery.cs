namespace Application.Features.Workouts.GetAll;

public record GetWorkoutsQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortOrder = null);
