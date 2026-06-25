using System.Net;
using System.Net.Http.Json;
using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using PublicApi.Endpoints.ScheduledWorkouts;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ScheduledWorkoutEndpoints;

[Collection("Database Shared Collection")]
public class RescheduleWorkoutTests : IAsyncLifetime
{
    private const string TimeZoneHeader = "X-TimeZone";

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public RescheduleWorkoutTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RescheduleWorkout_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("workout 1", "workout 1 description", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        workout.AddScheduledWorkout(scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        var zone = "Africa/Cairo";
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, zone);

        var sessionDate = DateTime.Now.AddDays(2);
        var request = new ScheduleWorkoutRequest()
        {
            SessionDate = sessionDate
        };

        var response = await _client.PostAsJsonAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/reschedule", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updatedScheduledWorkout = await dbContext.ScheduledWorkouts
            .SingleAsync(x => x.Id == scheduledWorkout.Id);

        var utcLocalConverter = scope.ServiceProvider.GetRequiredService<IUtcLocalConverter>();

        Assert.Equal(sessionDate, utcLocalConverter.ConvertUtcToLocal(updatedScheduledWorkout.SessionDate, zone));
    }

    [Fact]
    public async Task RescheduleWorkout_TryToRescheduleNotPendingScheduledWorkout_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("workout 1", "workout 1 description", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        workout.AddScheduledWorkout(scheduledWorkout);
        scheduledWorkout.Start();

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        var zone = "Africa/Cairo";
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, zone);

        var request = new ScheduleWorkoutRequest()
        {
            SessionDate = DateTime.Now.AddDays(2)
        };

        var response = await _client.PostAsJsonAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/reschedule", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RescheduleWorkout_TryToRescheduleInThePast_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("workout 1", "workout 1 description", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        workout.AddScheduledWorkout(scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        var zone = "Africa/Cairo";
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, zone);

        var request = new ScheduleWorkoutRequest()
        {
            SessionDate = DateTime.Now.AddDays(-1)
        };

        var response = await _client.PostAsJsonAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/reschedule", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
