using Application.Features.ScheduledWorkouts.GetAll;
using Domain.Entities;
using NodaTime;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace PublicApiIntegrationTests.ScheduledWorkoutEndpoints;

[Collection("Database Shared Collection")]
public class GetScheduledWorkoutsTests : IAsyncLifetime
{
    private const string TimeZoneHeader = "X-TimeZone";

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public GetScheduledWorkoutsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllScheduledWorkouts_WithValidRequest_ReturnsOk()
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

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/scheduled-workouts?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var scheduledWorkoutsResponse = await response.Content.ReadFromJsonAsync<GetScheduledWorkoutsResponse>();
        Assert.NotNull(scheduledWorkoutsResponse);

        Assert.Single(scheduledWorkoutsResponse.ScheduledWorkoutDtos);
    }

    [Fact]
    public async Task GetAllScheduledWorkouts_WithMultipleScheduledWorkouts_ReturnsPaginatedResults()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);
        workout.AddExercise(new Exercise("Push Ups", "3 sets", workout.Id));

        for (int i = 1; i <= 5; i++)
        {
            var scheduledWorkout = ScheduledWorkout.Schedule(
                workout,
                SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(i)));
            workout.AddScheduledWorkout(scheduledWorkout);
        }

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/scheduled-workouts?page=1&pageSize=2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var scheduledWorkoutsResponse = await response.Content.ReadFromJsonAsync<GetScheduledWorkoutsResponse>();
        Assert.NotNull(scheduledWorkoutsResponse);

        Assert.Equal(2, scheduledWorkoutsResponse.ScheduledWorkoutDtos.Count());
    }

    [Fact]
    public async Task GetAllScheduledWorkouts_WithSortOrder_ReturnsSortedResults()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);
        workout.AddExercise(new Exercise("Push Ups", "3 sets", workout.Id));

        var firstScheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));
        var secondScheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(2)));
        var thirdScheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(3)));

        workout.AddScheduledWorkout(firstScheduledWorkout);
        workout.AddScheduledWorkout(secondScheduledWorkout);
        workout.AddScheduledWorkout(thirdScheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/scheduled-workouts?page=1&pageSize=10&sortOrder=desc");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var scheduledWorkoutsResponse = await response.Content.ReadFromJsonAsync<GetScheduledWorkoutsResponse>();

        Assert.NotNull(scheduledWorkoutsResponse);
        Assert.Equal(3, scheduledWorkoutsResponse.ScheduledWorkoutDtos.Count());
        Assert.Equal(thirdScheduledWorkout.Id, scheduledWorkoutsResponse.ScheduledWorkoutDtos.First().Id);
    }

    [Fact]
    public async Task GetAllScheduledWorkouts_ReturnsOnlyCurrentUserScheduledWorkouts()
    {
        // Arrange
        var user1 = new User("TestUser1", "test1@example.com", "hashedpassword");
        var user2 = new User("TestUser2", "test2@example.com", "hashedpassword");

        var workout1 = new Workout("User1 Workout", "U1", user1.Id);
        workout1.AddExercise(new Exercise("User1 Exercise", "U1", workout1.Id));

        var workout2 = new Workout("User2 Workout", "U2", user2.Id);
        workout2.AddExercise(new Exercise("User2 Exercise", "U2", workout2.Id));

        var scheduledWorkout1 = ScheduledWorkout.Schedule(
            workout1,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));
        var scheduledWorkout2 = ScheduledWorkout.Schedule(
            workout2,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        workout1.AddScheduledWorkout(scheduledWorkout1);
        workout2.AddScheduledWorkout(scheduledWorkout2);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user1);
            await dbContext.Users.AddAsync(user2);
            await dbContext.Workouts.AddAsync(workout1);
            await dbContext.Workouts.AddAsync(workout2);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user1);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout1.Id}/scheduled-workouts?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var scheduledWorkoutsResponse = await response.Content.ReadFromJsonAsync<GetScheduledWorkoutsResponse>();

        Assert.NotNull(scheduledWorkoutsResponse);
        Assert.Single(scheduledWorkoutsResponse.ScheduledWorkoutDtos);
        Assert.Equal(scheduledWorkout1.Id, scheduledWorkoutsResponse.ScheduledWorkoutDtos.ElementAt(0).Id);
    }

    [Fact]
    public async Task GetAllScheduledWorkouts_NotProvideTimezoneHeader_ReturnsBadRequest()
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

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/scheduled-workouts?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
