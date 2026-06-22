using System.Net.Http.Json;
using Application.Abstraction;
using Application.Features.Authentication.Signup;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PublicApi.Endpoints.Authentication.Signup;

namespace PublicApiIntegrationTests.AuthEndpoints;

public class SignupTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SignupTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    } 

    [Fact]
    public async Task Signup_WithValidRequest_ReturnsOk()
    {
        await _factory.ResetDatabase();
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

        var body = await response.Content.ReadFromJsonAsync<SignupResult>();
        Assert.Equal(3, body!.Token.Split('.').Count());
    }

    [Fact]
    public async Task Signup_WithDuplicateEmail_ReturnsConflict()
    {
        await _factory.ResetDatabase();

        using var scope = _factory.Services.CreateScope();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await _factory.SeedAsync(async dbContext =>
        {
            var user = new User("test", "test@gmail.com", passwordHasher.HashPassword("Test@1234"));

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        });

        // Arrange
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

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        Assert.NotNull(user);
    }
}
