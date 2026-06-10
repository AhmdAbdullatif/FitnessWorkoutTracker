using System.Text;
using Application.Abstraction;
using Application.Features.Authentication.Login;
using Application.Features.Authentication.Signup;
using Application.Features.Exercises.Create;
using Application.Features.Exercises.Get;
using Application.Features.ScheduledWorkouts.Schedule;
using Application.Features.Workouts.Create;
using Application.Features.Workouts.GetAll;
using Domain.Entities;
using FastEndpoints;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.Services.Authentication;
using Infrastructure.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PublicApi;
using PublicApi.Middlewares;
using PublicApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<SignupUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
builder.Services.AddScoped<CreateWorkoutUseCase>();
builder.Services.AddScoped<GetWorkoutsUseCase>();

builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<CreateExerciseUseCase>();

builder.Services.AddScoped<GetExercisesUseCases>();

builder.Services.AddSingleton<IUtcLocalConverter, UtcLocalConverter>();

builder.Services.AddScoped<ScheduleWorkoutUseCase>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseFastEndpoints();

app.Run();
