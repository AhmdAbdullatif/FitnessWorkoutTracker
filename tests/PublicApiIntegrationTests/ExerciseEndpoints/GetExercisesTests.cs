using Application.Features.Exercises.GetAll;
using Domain.Entities;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace PublicApiIntegrationTests.ExerciseEndpoints;

[Collection("Database Shared Collection")]
public class GetExercisesTests : IAsyncLifetime
{
    private const string TimeZoneHeader = "X-TimeZone";

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public GetExercisesTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllExercises_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);
        var exercise = new Exercise("Push Ups", "3 sets", workout.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.Exercises.AddAsync(exercise);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/exercises?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercisesResponse = await response.Content.ReadFromJsonAsync<GetExercisesResponse>();
        Assert.NotNull(exercisesResponse);

        Assert.Single(exercisesResponse.ExerciseDtos);
    }

    [Fact]
    public async Task GetAllExercises_WithMultipleExercises_ReturnsPaginatedResults()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);

            for (int i = 1; i <= 5; i++)
            {
                var exercise = new Exercise($"Exercise {i}", $"Description {i}", workout.Id);
                await dbContext.Exercises.AddAsync(exercise);
            }

            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/exercises?page=1&pageSize=3");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercisesResponse = await response.Content.ReadFromJsonAsync<GetExercisesResponse>();
        Assert.NotNull(exercisesResponse);

        Assert.Equal(3, exercisesResponse.ExerciseDtos.Count());
    }

    [Fact]
    public async Task GetAllExercises_WithSearchTerm_FiltersExercises()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);

            var exercise1 = new Exercise("Bench Press", "Chest exercise", workout.Id);
            var exercise2 = new Exercise("Squat", "Leg exercise", workout.Id);
            var exercise3 = new Exercise("Running Sprint", "Cardio exercise", workout.Id);

            await dbContext.Exercises.AddAsync(exercise1);
            await dbContext.Exercises.AddAsync(exercise2);
            await dbContext.Exercises.AddAsync(exercise3);

            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/exercises?page=1&pageSize=10&searchTerm=Running");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercisesResponse = await response.Content.ReadFromJsonAsync<GetExercisesResponse>();
        Assert.NotNull(exercisesResponse);
        Assert.Single(exercisesResponse.ExerciseDtos);
        Assert.True(exercisesResponse.ExerciseDtos.All(e => e.Title.Contains("Running")));
    }

    [Fact]
    public async Task GetAllExercises_WithSortOrder_ReturnsSortedResults()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);

            var exercises = new[]
            {
                new Exercise("Exercise 1", "1", workout.Id),
                new Exercise("Exercise 2", "2", workout.Id),
                new Exercise("Exercise 3", "3", workout.Id)
            };

            foreach (var exercise in exercises)
            {
                await dbContext.Exercises.AddAsync(exercise);
            }

            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout.Id}/exercises?page=1&pageSize=10&sortOrder=desc");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var exercisesResponse = await response.Content.ReadFromJsonAsync<GetExercisesResponse>();

        Assert.NotNull(exercisesResponse);
        Assert.Equal(3, exercisesResponse.ExerciseDtos.Count());
        Assert.Equal("Exercise 3", exercisesResponse.ExerciseDtos.First().Title);
    }

    [Fact]
    public async Task GetAllExercises_ReturnsOnlyCurrentUserExercises()
    {
        // Arrange
        var user1 = DataSeedHelper.CreateUser();
        var user2 = DataSeedHelper.CreateUser();

        var workout1 = new Workout("User1 Workout", "U1", user1.Id);
        var workout2 = new Workout("User2 Workout", "U2", user2.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user1);
            await dbContext.Users.AddAsync(user2);

            await dbContext.Workouts.AddAsync(workout1);
            await dbContext.Workouts.AddAsync(workout2);

            var exercise1 = new Exercise("User1 Exercise", "U1", workout1.Id);
            var exercise2 = new Exercise("User2 Exercise", "U2", workout2.Id);

            await dbContext.Exercises.AddAsync(exercise1);
            await dbContext.Exercises.AddAsync(exercise2);

            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user1);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.GetAsync($"api/workouts/{workout1.Id}/exercises?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercisesResponse = await response.Content.ReadFromJsonAsync<GetExercisesResponse>();

        Assert.NotNull(exercisesResponse);
        Assert.Single(exercisesResponse.ExerciseDtos);
        Assert.Equal("U1", exercisesResponse.ExerciseDtos.ElementAt(0).Description);
    }
}
