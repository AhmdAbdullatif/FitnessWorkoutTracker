using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using Application.Features.ExerciseProgresses.Start;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ExerciseProgressEndpoints;

[Collection("Database Shared Collection")]
public class StartExerciseProgressTests : IAsyncLifetime
{
    private const string TimeZoneHeader = "X-TimeZone";
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public StartExerciseProgressTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task StartExerciseProgress_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);
        workout.AddExercise(new Exercise("Exercise 1", "Description 1", workout.Id));

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

        var request = new StartExerciseRequest
        {
            Sets = 3,
            Reps = 12
        };

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.PostAsJsonAsync($"api/exercise-progresses/{exerciseProgressId}/start", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var startExerciseResponse = await response.Content.ReadFromJsonAsync<StartExerciseResponse>();
        Assert.NotNull(startExerciseResponse);
        Assert.Equal(exerciseProgressId, startExerciseResponse.Id);
        Assert.Equal(3, startExerciseResponse.Sets);
        Assert.Equal(12, startExerciseResponse.Reps);
        Assert.Equal(ExerciseStatus.InProgress, startExerciseResponse.Status);
        Assert.NotEqual(default, startExerciseResponse.StartedAt);
    }


    [Fact]
    public async Task StartExerciseProgress_TryToStartNotPendingExerciseProgress_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);
        workout.AddExercise(new Exercise("Exercise 1", "Description 1", workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Start();
        workout.AddScheduledWorkout(scheduledWorkout);

        var exerciseProgress = scheduledWorkout.ExerciseProgresses.First();
        exerciseProgress.Start(3, 12);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        var request = new StartExerciseRequest
        {
            Sets = 1,
            Reps = 50
        };

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        _client.DefaultRequestHeaders.Add(TimeZoneHeader, "UTC");

        var response = await _client.PostAsJsonAsync($"api/exercise-progresses/{exerciseProgress.Id}/start", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var exerciseProgressTest = await dbContext.ExerciseProgresses
            .SingleAsync(x => x.Id == exerciseProgress.Id);

        Assert.NotEqual(request.Sets, exerciseProgressTest.Sets);
        Assert.NotEqual(request.Reps, exerciseProgressTest.Reps);
    }
    
}
