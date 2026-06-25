using Application.Features.ExerciseProgresses.AddNote;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using PublicApiIntegrationTests.Extensions;
using PublicApiIntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace PublicApiIntegrationTests.ExerciseProgressEndpoints;

[Collection("Database Shared Collection")]
public class AddNoteToExerciseProgressTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _client;

    public AddNoteToExerciseProgressTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddNoteToExerciseProgress_WithValidRequest_ReturnsOkAndAddsNote()
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
        var request = new AddNoteRequest
        {
            Content = "Felt strong today"
        };

        // Act
        _client = _factory.CreateAuthenticatedClient(user);

        var response = await _client.PostAsJsonAsync($"api/exercise-progresses/{exerciseProgressId}/notes", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var note = await dbContext.Notes
            .SingleAsync(x => x.ExerciseProgressId == exerciseProgressId);

        Assert.Equal(request.Content, note.Content);
    }

}
