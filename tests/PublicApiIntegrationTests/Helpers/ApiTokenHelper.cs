using Application.Abstraction;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace PublicApiIntegrationTests.Helpers;

public class ApiTokenHelper
{
    public static string GetToken(CustomWebApplicationFactory factory, User user)
    {
        using var scope = factory.Services.CreateScope();

        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();

        var token = jwtProvider.CreateAccessToken(user.Id, user.Email);
        return token;
    }
}
