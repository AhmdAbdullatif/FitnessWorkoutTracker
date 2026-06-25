using System.Net;
using System.Net.Http.Json;
using Application.Features.ScheduledWorkouts.GetById;
using Domain.Entities;
using NodaTime;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ScheduledWorkoutEndpoints;

[Collection("Database Shared Collection")]
public class ScheduledWorkoutByIdTests : IAsyncLifetime
{
    private const string TimeZoneHeader = "X-TimeZone";

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public ScheduledWorkoutByIdTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetScheduledWorkoutById_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);
        workout.AddExercise(new Exercise("Push Ups", "3 sets", workout.Id));
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
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/scheduled-workouts/{scheduledWorkout.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var scheduledWorkoutDto = await response.Content.ReadFromJsonAsync<ScheduledWorkoutDto>();

        Assert.NotNull(scheduledWorkoutDto);
        Assert.Equal(workout.Title, scheduledWorkoutDto.Title);
    }

    [Fact]
    public async Task GetScheduledWorkoutById_TryToGetAnotherUserScheduledWorkout_ReturnsNotFound()
    {
        // Arrange
        var user1 = DataSeedHelper.CreateUser();
        var workout1 = new Workout("Morning Run", "5km run", user1.Id);

        workout1.AddExercise(new Exercise("Push Ups", "3 sets", workout1.Id));
        var scheduledWorkout1 = ScheduledWorkout.Schedule(
            workout1,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));
        workout1.AddScheduledWorkout(scheduledWorkout1);

        var user2 = DataSeedHelper.CreateUser();
        var workout2 = new Workout("Morning Run", "5km run", user2.Id);

        workout2.AddExercise(new Exercise("pull ups", "2 sets", workout2.Id));
        var scheduledWorkout2 = ScheduledWorkout.Schedule(
            workout2,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(2)));
        workout2.AddScheduledWorkout(scheduledWorkout2);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user1);
            await dbContext.Workouts.AddAsync(workout1);

            await dbContext.Users.AddAsync(user2);
            await dbContext.Workouts.AddAsync(workout2);
            
            await dbContext.SaveChangesAsync();
        });
        // Act
        _client = _factory.CreateAuthenticatedClient(user1);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/scheduled-workouts/{scheduledWorkout2.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
