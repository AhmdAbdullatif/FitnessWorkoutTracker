using System.Net.Http.Json;
using Application.Abstraction;
using Application.Features.Authentication;
using Application.Features.Authentication.Signup;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PublicApi.Endpoints.Authentication.Signup;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.AuthEndpoints;

[Collection("Database Shared Collection")]
public class SignupTests : IAsyncLifetime
{
    private CustomWebApplicationFactory _factory;
    private HttpClient _client;
    private IServiceScope CreateScope() => _factory.Services.CreateScope();
    public SignupTests(CustomWebApplicationFactory factory) 
    {
        _factory = factory;
        _client = _factory.CreateClient();        
    }
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Signup_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new SignupRequest()
        {
            Username = "ahmed",
            Email = "ahmed@gmail.com",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/signup", request);

        // Assert 
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        
        Assert.NotNull(user);
        Assert.Equal(request.Email, user.Email);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthenticateResponse>();
        Assert.Equal(3, authResponse!.AccessToken.Split('.').Count());
        Assert.NotNull(authResponse.RefreshToken);
    }

    [Fact]
    public async Task Signup_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        await _factory.SeedAsync(async dbContext =>
        {
            var user = DataSeedHelper.CreateUser();

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        });

        var request = new SignupRequest()
        {
            Username = "test",
            Email = "test@gmail.com",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/auth/signup", request);

        // Assert 
        Assert.Equal(StatusCodes.Status409Conflict, (int)response.StatusCode);

        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        Assert.NotNull(user);
    }
}
