using Application.Abstraction;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace PublicApiIntegrationTests.Helpers;

public class ApiTokenHelper
{
    public static string GetToken(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();

        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        Guid userId = Guid.NewGuid();
        string email = "test@gmail.com";

        var token = jwtProvider.Create(userId, email);
        return token;
    }
}
