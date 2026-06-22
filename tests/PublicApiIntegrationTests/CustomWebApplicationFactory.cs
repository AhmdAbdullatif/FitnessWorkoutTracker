using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PublicApiIntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.RemoveAll<AppDbContext>();

            _connection =
                new SqliteConnection(
                    "DataSource=:memory:");

            _connection.Open();

            services.AddDbContext<AppDbContext>(
                options =>
                {
                    options.UseSqlite(
                        _connection);
                });

            var provider =
                services.BuildServiceProvider();

            using var scope =
                provider.CreateScope();

            var dbContext =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.EnsureCreated();
        });
    }

    public async Task ResetDatabase()
    {
        using var scope =
            Services.CreateScope();

        var dbContext =
            scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    public async Task SeedAsync(
        Func<AppDbContext, Task> action)
    {
        using var scope =
            Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        await action(db);
    }
}
