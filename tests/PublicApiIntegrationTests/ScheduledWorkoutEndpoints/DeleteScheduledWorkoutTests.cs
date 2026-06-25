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
public class DeleteScheduledWorkoutTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public DeleteScheduledWorkoutTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteScheduledWorkout_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var user = DataSeedHelper.CreateUser();

        var workout = new Workout("Workout 1", "U1", user.Id);

        workout.AddExercise(new Exercise("Exercise 1", null, workout.Id));

        var scheduledWorkout = ScheduledWorkout.Schedule(workout,
            SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(1)));

        scheduledWorkout.Start();
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

        var response = await _client.DeleteAsync($"api/scheduled-workouts/{scheduledWorkout.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var scheduledWorkoutExists = await dbContext.ScheduledWorkouts
           .AnyAsync(x => x.Id == scheduledWorkout.Id);

        Assert.False(scheduledWorkoutExists);
    }
}
