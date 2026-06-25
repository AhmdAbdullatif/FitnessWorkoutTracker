using System.Net;
using System.Net.Http.Json;
using Application.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using PublicApi.Endpoints.ScheduledWorkouts;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ScheduledWorkoutEndpoints;

[Collection("Database Shared Collection")]
public class ScheduleWorkoutTests : IAsyncLifetime
{
    private const string TimeZoneHeader = "X-TimeZone";

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public ScheduleWorkoutTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ScheduleWorkout_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("workout 1", "workout 1 description", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

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

        var sessionDate = DateTime.Now.AddDays(1);
        var request = new ScheduleWorkoutRequest()
        {
            SessionDate = sessionDate
        };

        var response = await _client.PostAsJsonAsync($"api/workouts/{workout.Id}/scheduled-workouts", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var scope = _factory.Services.CreateScope();

        var utcLocalConverter = scope.ServiceProvider.GetRequiredService<IUtcLocalConverter>();
        var sessionInstant = utcLocalConverter.ConvertLocalToUtc(sessionDate, zone);

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var scheduledWorkouts = dbContext.ScheduledWorkouts
            .Where(x => x.WorkoutId == workout.Id);

        Assert.NotEmpty(scheduledWorkouts);
        Assert.True(scheduledWorkouts.Any(x => x.SessionDate == sessionInstant));
    }

    [Fact]
    public async Task ScheduleWorkout_TryToScheduleAWorkoutWithoutExercises_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("workout 1", "workout 1 description", user.Id); // no exercises

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

        var sessionDate = DateTime.Now.AddDays(1);
        var request = new ScheduleWorkoutRequest()
        {
            SessionDate = sessionDate
        };

        var response = await _client.PostAsJsonAsync($"api/workouts/{workout.Id}/scheduled-workouts", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var scheduledWorkouts = dbContext.ScheduledWorkouts
            .Where(x => x.WorkoutId == workout.Id);

        Assert.Empty(scheduledWorkouts);
    }

    [Fact]
    public async Task ScheduleWorkout_TryToScheduleAWorkoutInThePast_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("workout 1", "workout 1 description", user.Id);

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

        var sessionDate = new DateTime(2026, 1, 1); // past date
        var request = new ScheduleWorkoutRequest()
        {
            SessionDate = sessionDate
        };

        var response = await _client.PostAsJsonAsync($"api/workouts/{workout.Id}/scheduled-workouts", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var scheduledWorkouts = dbContext.ScheduledWorkouts
            .Where(x => x.WorkoutId == workout.Id);

        Assert.Empty(scheduledWorkouts);
    }

}
