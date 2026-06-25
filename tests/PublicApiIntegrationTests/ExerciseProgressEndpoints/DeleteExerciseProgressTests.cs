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
public class DeleteExerciseProgressTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public DeleteExerciseProgressTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task DeleteExerciseProgress_WithExistingExerciseProgress_ReturnsNoContentAndDeletesExerciseProgress()
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

        await _factory.SeedAsync(async dbContext =>
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.Workouts.AddAsync(workout);
            await dbContext.SaveChangesAsync();
        });

        var exerciseProgressId = scheduledWorkout.ExerciseProgresses.First().Id;

        // Act
        _client = _factory.CreateAuthenticatedClient(user);

        var response = await _client.DeleteAsync($"api/exercise-progresses/{exerciseProgressId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deletedExerciseProgress = await dbContext.ExerciseProgresses
            .FirstOrDefaultAsync(x => x.Id == exerciseProgressId);

        Assert.Null(deletedExerciseProgress);
    }
    
}
