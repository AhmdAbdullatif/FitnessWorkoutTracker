using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;
using System.Net;

namespace PublicApiIntegrationTests.ExerciseProgressEndpoints;

[Collection("Database Shared Collection")]
public class CompleteExerciseProgressTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public CompleteExerciseProgressTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CompleteExerciseProgress_WithValidRequest_ReturnsNoContentAndMarksExerciseProgressAsCompleted()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);
        workout.AddExercise(new Exercise("Exercise 1", "Description 1", workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(
            workout,
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

        // Act
        _client = _factory.CreateAuthenticatedClient(user);

        var response = await _client.PostAsync($"api/exercise-progresses/{exerciseProgress.Id}/complete", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updatedExerciseProgress = await dbContext.ExerciseProgresses
            .SingleAsync(x => x.Id == exerciseProgress.Id);

        Assert.Equal(ExerciseStatus.Completed, updatedExerciseProgress.Status);
        Assert.NotNull(updatedExerciseProgress.CompletedAt);
    }

    [Fact]
    public async Task CompleteExerciseProgress_TryToCompleteExerciseProgressThatIsNotInProgress_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);
        workout.AddExercise(new Exercise("Exercise 1", "Description 1", workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Start();
        workout.AddScheduledWorkout(scheduledWorkout);

        var exerciseProgress = scheduledWorkout.ExerciseProgresses.First();

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);

        var response = await _client.PostAsync($"api/exercise-progresses/{exerciseProgress.Id}/complete", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updatedExerciseProgress = await dbContext.ExerciseProgresses
            .SingleAsync(x => x.Id == exerciseProgress.Id);

        Assert.Equal(ExerciseStatus.Pending, updatedExerciseProgress.Status);
        Assert.Null(updatedExerciseProgress.CompletedAt);
    }

    [Fact]
    public async Task CompleteExerciseProgress_TryToCompleteWhenScheduledWorkoutIsNotInProgress_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);
        var exercise = new Exercise("Exercise 1", "Description 1", workout.Id);
        workout.AddExercise(exercise);

        var scheduledWorkout = ScheduledWorkout.Schedule(
            workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        workout.AddScheduledWorkout(scheduledWorkout);

        var exerciseProgress = new ExerciseProgress(exercise.Id, scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.ExerciseProgresses.AddAsync(exerciseProgress);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);

        var response = await _client.PostAsync($"api/exercise-progresses/{exerciseProgress.Id}/complete", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updatedExerciseProgress = await dbContext.ExerciseProgresses
            .SingleAsync(x => x.Id == exerciseProgress.Id);

        Assert.Equal(ExerciseStatus.Pending, updatedExerciseProgress.Status);
        Assert.Null(updatedExerciseProgress.CompletedAt);
    }
}
