

using System.Net;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ScheduledWorkoutEndpoints;

[Collection("Database Shared Collection")]
public class StartScheduledWorkoutTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public StartScheduledWorkoutTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task StartScheduledWorkout_WithValidRequest_ReturnsNoContent()
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

        var response = await _client.PostAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/start", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updatedScheduledWorkout = await dbContext.ScheduledWorkouts
            .FirstOrDefaultAsync(x => x.Id == scheduledWorkout.Id);

        Assert.NotNull(updatedScheduledWorkout);
        Assert.Equal(WorkoutStatus.InProgress, updatedScheduledWorkout.Status);

        var exerciseProgresses = dbContext.ExerciseProgresses.Where(x => x.ScheduledWorkoutId == scheduledWorkout.Id);
        Assert.NotEmpty(exerciseProgresses);
    }

    [Fact]
    public async Task StartScheduledWorkout_TryToStartNotPendingScheduledWorkout_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();
        var workout = new Workout("Morning Run", "5km run", user.Id);

        workout.AddExercise(new Exercise("Push Ups", "3 sets", workout.Id));

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
        _client = _factory.CreateAuthenticatedClient(user);

        var response = await _client.PostAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/start", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
