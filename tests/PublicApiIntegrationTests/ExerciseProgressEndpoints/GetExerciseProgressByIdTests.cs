using System.Net;
using System.Net.Http.Json;
using Application.Features.ExerciseProgresses.GetById;
using Domain.Entities;
using NodaTime;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ExerciseProgressEndpoints;

[Collection("Database Shared Collection")]
public class GetExerciseProgressByIdTests : IAsyncLifetime
{
    private const string TimeZoneHeader = "X-TimeZone";
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public GetExerciseProgressByIdTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetExerciseProgressById_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);
        var exercise = new Exercise("Exercise 1", "Description 1", workout.Id);
        workout.AddExercise(exercise);

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Start();
        workout.AddScheduledWorkout(scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        var exerciseProgressId = scheduledWorkout.ExerciseProgresses.First().Id;

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/exercise-progresses/{exerciseProgressId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getExerciseProgressByIdResponse = await response.Content.ReadFromJsonAsync<GetExerciseProgressByIdResponse>();
        Assert.NotNull(getExerciseProgressByIdResponse);
        Assert.Equal(exerciseProgressId, getExerciseProgressByIdResponse.Id);
        Assert.Equal("Exercise 1", getExerciseProgressByIdResponse.Title);
        Assert.Equal("Description 1", getExerciseProgressByIdResponse.Description);
        Assert.Equal(ExerciseStatus.Pending, getExerciseProgressByIdResponse.Status);
        Assert.Null(getExerciseProgressByIdResponse.StartedAt);
        Assert.Empty(getExerciseProgressByIdResponse.Notes);
    }

    [Fact]
    public async Task GetExerciseProgressById_TryToAccessAnotherUserExerciseProgress_ReturnsNotFound()
    {
        // Arrange
        var user1 = DataSeedHelper.CreateUser();
        var user2 = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user1.Id);
        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Start();
        workout.AddScheduledWorkout(scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user1);
            await dbContext.Users.AddAsync(user2);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        var exerciseProgressId = scheduledWorkout.ExerciseProgresses.First().Id;

        // Act
        _client = _factory.CreateAuthenticatedClient(user2);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/exercise-progresses/{exerciseProgressId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetExerciseProgressById_NotProvideTimezoneHeader_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);
        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Start();
        workout.AddScheduledWorkout(scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        var exerciseProgressId = scheduledWorkout.ExerciseProgresses.First().Id;

        // Act
        _client = _factory.CreateAuthenticatedClient(user);

        var response = await _client.GetAsync($"api/exercise-progresses/{exerciseProgressId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
