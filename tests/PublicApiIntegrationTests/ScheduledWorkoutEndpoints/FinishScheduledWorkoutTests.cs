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
public class FinishScheduledWorkoutTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public FinishScheduledWorkoutTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task FinishScheduledWorkout_FinishInProgressScheduledWorkout_ReturnsNoContent()
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

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        
        var response = await _client.PostAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/finish", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var finishedScheduledWorkout = await dbContext.ScheduledWorkouts
            .Include(x => x.ExerciseProgresses)
            .SingleAsync(x => x.Id == scheduledWorkout.Id);

        Assert.Equal(WorkoutStatus.Completed, finishedScheduledWorkout.Status);
        Assert.True(finishedScheduledWorkout.ExerciseProgresses.All(x => x.Status == ExerciseStatus.Skipped));
    }

    [Fact]
    public async Task FinishScheduledWorkout_TryFinishPendingScheduledWorkout_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
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
        
        var response = await _client.PostAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/finish", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var finishedScheduledWorkout = await dbContext.ScheduledWorkouts
            .Include(x => x.ExerciseProgresses)
            .SingleAsync(x => x.Id == scheduledWorkout.Id);

        Assert.Equal(WorkoutStatus.Pending, finishedScheduledWorkout.Status);
        Assert.Empty(finishedScheduledWorkout.ExerciseProgresses);
    }

    [Fact]
    public async Task FinishScheduledWorkout_TryFinishCanceledScheduledWorkout_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Cancel();
        workout.AddScheduledWorkout(scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        
        var response = await _client.PostAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/finish", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var finishedScheduledWorkout = await dbContext.ScheduledWorkouts
            .SingleAsync(x => x.Id == scheduledWorkout.Id);

        Assert.Equal(WorkoutStatus.Canceled, finishedScheduledWorkout.Status);
    }

    [Fact]
    public async Task FinishScheduledWorkout_TryFinishCompletedScheduledWorkout_ReturnsBadRequest()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Start();
        scheduledWorkout.Finish();

        workout.AddScheduledWorkout(scheduledWorkout);

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        // Act
        _client = _factory.CreateAuthenticatedClient(user);
        
        var response = await _client.PostAsync($"api/scheduled-workouts/{scheduledWorkout.Id}/finish", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var finishedScheduledWorkout = await dbContext.ScheduledWorkouts
            .SingleAsync(x => x.Id == scheduledWorkout.Id);

        Assert.Equal(WorkoutStatus.Completed, finishedScheduledWorkout.Status);
    }

}
