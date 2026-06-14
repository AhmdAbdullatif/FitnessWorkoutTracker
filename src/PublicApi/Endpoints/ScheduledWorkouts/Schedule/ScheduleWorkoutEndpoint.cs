using Application.Features.ScheduledWorkouts.Schedule;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.ScheduledWorkouts;

public class ScheduleWorkoutEndpoint(IScheduleWorkoutUseCase scheduleWorkoutUseCase) : Endpoint<ScheduleWorkoutRequest, ScheduleWorkoutResponse>
{
    public override void Configure()
    {
        Post("api/workouts/{workoutId}/scheduled-workouts");
    }

    public override async Task HandleAsync(ScheduleWorkoutRequest req, CancellationToken ct)
    {
        var userZone = HttpContext.Request.Headers[HeaderNames.TIME_ZONE_HEADER].ToString();

        var workoutId = Route<Guid>("workoutId");

        var response = await scheduleWorkoutUseCase.ExecuteAsync(req.SessionDate, workoutId, userZone);

        await SendAsync(response, cancellation: ct);
    }

}
