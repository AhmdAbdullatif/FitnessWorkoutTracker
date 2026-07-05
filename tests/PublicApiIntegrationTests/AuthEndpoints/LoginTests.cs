using Application.Abstraction;
using Application.Features.Authentication;
using Application.Features.Authentication.Login;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using PublicApiIntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace PublicApiIntegrationTests.AuthEndpoints;


[Collection("Database Shared Collection")]
public class LoginTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly IPasswordHasher _passwordHasher;
    private IServiceScope CreateScope() => _factory.Services.CreateScope();

    public LoginTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        using var scope = CreateScope();
        _passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    }
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_WithValidRequest_ReturnsOkWithToken()
    {
        // Arrange
        await _factory.SeedAsync(async dbContext =>
        {
            User user = DataSeedHelper.CreateLoginUser(_passwordHasher);
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        });

        // Act
        var request = new LoginCommand("test@gmail.com", "Test@1234");

        var response = await _client.PostAsJsonAsync("api/auth/login", request);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthenticateResponse>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        Assert.NotNull(authResponse);

        Assert.Equal(3, authResponse!.AccessToken.Split('.').Length);
        Assert.NotNull(authResponse.RefreshToken);
    }

    [Fact]
    public async Task Login_WithWrongEmail_ReturnsBadRequest()
    {
        // Act
        var request = new LoginCommand("test@gmail.com", "Test@1234");

        var response = await _client.PostAsJsonAsync("api/auth/login", request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsOkWithToken()
    {
        // Arrange
        await _factory.SeedAsync(async dbContext =>
        {
            User user = DataSeedHelper.CreateLoginUser(_passwordHasher);
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        });

        // Act
        var request = new LoginCommand("test@gmail.com", "Rest1234");

        var response = await _client.PostAsJsonAsync("api/auth/login", request);
        // Assert
        Assert.False(response.IsSuccessStatusCode);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

