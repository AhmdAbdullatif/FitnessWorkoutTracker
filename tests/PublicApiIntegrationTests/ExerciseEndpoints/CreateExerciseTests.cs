using Application.Features.Exercises.Create;
using Domain.Entities;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace PublicApiIntegrationTests.ExerciseEndpoints;

[Collection("Database Shared Collection")]
public class CreateExerciseTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public CreateExerciseTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateExercise_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Workout 1", "U1", user.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        var command = new CreateExerciseRequest()
        {
            Title = "Exercise 1",
            Description = "U1"
        };

        var response = await _client.PostAsJsonAsync($"api/workouts/{workout.Id}/exercises", command);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createResponse = await response.Content.ReadFromJsonAsync<CreateExerciseResponse>();

        Assert.NotNull(createResponse);
        Assert.Equal(command.Title, createResponse.Title);
        Assert.Equal(command.Description, createResponse.Description);
        Assert.Equal(workout.Id, createResponse.WorkoutId);
        Assert.NotEqual(Guid.Empty, createResponse.Id);
    }

    [Fact]
    public async Task CreateExercise_NotProvideTitle_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Workout 1", "U1", user.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        var command = new CreateExerciseRequest()
        {
            Description = "U1"
        };

        var response = await _client.PostAsJsonAsync($"api/workouts/{workout.Id}/exercises", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateExercise_TryToCreateExerciseForAnotherUserWorkout_ReturnsNotFound()
    {
        // Arrange
        var user1 = new User("TestUser1", "test1@example.com", "hashedpassword");
        var user2 = new User("TestUser2", "test2@example.com", "hashedpassword");
        var workout = new Workout("User2 Workout", "U2", user2.Id);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user1);
            await dbContext.Users.AddAsync(user2);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user1);
        var command = new CreateExerciseRequest()
        {
            Title = "Exercise 1",
            Description = "U1"
        };

        var response = await _client.PostAsJsonAsync($"api/workouts/{workout.Id}/exercises", command);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
