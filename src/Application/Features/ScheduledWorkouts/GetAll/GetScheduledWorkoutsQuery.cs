namespace Application.Features.ScheduledWorkouts.GetAll;

public record GetScheduledWorkoutsQuery(int Page = 1, int PageSize = 10, string? SortOrder = null);