using FastEndpoints;
using FastEndpoints.Swagger;
using HealthChecks.UI.Client;
using Infrastructure.Data;
using Infrastructure.Services.Authentication;
using Infrastructure.Services.BackgroundServices;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using PublicApi;
using PublicApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders()
    .AddConsole()
    .AddDebug();

builder.Services.AddFastEndpoints()
    .SwaggerDocument(options =>
    {
        options.DocumentSettings = s =>
        {
            s.Title = "Fitness Workout Tracker API";
            s.Version = "v1";
        };
        options.EnableJWTBearerAuth = true;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddJwt(jwtOptions);

builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString);

builder.Services.AddCustomServices();

builder.Services.AddHostedService<RefreshTokenCleanupService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseAuthentication();

app.UseAuthorization();

app.UseFastEndpoints();

app.UseSwaggerGen();

app.Run();
