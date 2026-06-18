using System.Text;
using Application.Abstraction;
using Application.Features.Authentication.Login;
using Application.Features.Authentication.Signup;
using Application.Features.ExerciseProgresses.AddNote;
using Application.Features.ExerciseProgresses.Complete;
using Application.Features.ExerciseProgresses.Delete;
using Application.Features.ExerciseProgresses.GetAll;
using Application.Features.ExerciseProgresses.GetById;
using Application.Features.ExerciseProgresses.Skip;
using Application.Features.ExerciseProgresses.Start;
using Application.Features.Exercises.Create;
using Application.Features.Exercises.GetAll;
using Application.Features.ScheduledWorkouts.Cancel;
using Application.Features.ScheduledWorkouts.Delete;
using Application.Features.ScheduledWorkouts.Finish;
using Application.Features.ScheduledWorkouts.GetById;
using Application.Features.ScheduledWorkouts.Reschedule;
using Application.Features.ScheduledWorkouts.Schedule;
using Application.Features.ScheduledWorkouts.Start;
using Application.Features.Workouts.Create;
using Application.Features.Workouts.Delete;
using Application.Features.Workouts.GetAll;
using Application.Features.Workouts.GetById;
using Application.Features.Workouts.Update;
using FastEndpoints;
using Infrastructure.Data;
using Infrastructure.Logging;
using Infrastructure.Services;
using Infrastructure.Services.Authentication;
using Infrastructure.Services.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PublicApi.Middlewares;
using PublicApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders()
    .AddConsole()
    .AddDebug();

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

builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

builder.Services.AddScoped<ISignupUseCase, SignupUseCase>();
builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
builder.Services.AddScoped<ICreateWorkoutUseCase, CreateWorkoutUseCase>();
builder.Services.AddScoped<IGetWorkoutsUseCase, GetWorkoutsUseCase>();

builder.Services.AddScoped<ICreateExerciseUseCase, CreateExerciseUseCase>();

builder.Services.AddScoped<IGetExercisesUseCase, GetExercisesUseCase>();

builder.Services.AddSingleton<IUtcLocalConverter, UtcLocalConverter>();

builder.Services.AddScoped<IScheduleWorkoutUseCase, ScheduleWorkoutUseCase>();

builder.Services.AddScoped<IStartScheduledWorkoutUseCase, StartScheduledWorkoutUseCase>();

builder.Services.AddScoped<IStartExerciseProgressUseCase, StartExerciseProgressUseCase>();
builder.Services.AddScoped<IDeleteExerciseProgressUseCase, DeleteExerciseProgressUseCase>();

builder.Services.AddScoped<IGetExerciseProgressesUseCase, GetExerciseProgressesUseCase>();

builder.Services.AddScoped<IAddNoteToExerciseProgressUseCase, AddNoteToExerciseProgressUseCase>();

builder.Services.AddScoped<IGetExerciseProgressByIdUseCase, GetExerciseProgressByIdUseCase>();

builder.Services.AddScoped<IGetScheduledWorkoutByIdUseCase, GetScheduledWorkoutByIdUseCase>();

builder.Services.AddScoped<IFinishScheduledWorkoutUseCase, FinishScheduledWorkoutUseCase>();

builder.Services.AddScoped<ICancelScheduledWorkoutUseCase, CancelScheduledWorkoutUseCase>();

builder.Services.AddScoped<ISkipExerciseProgressUseCase, SkipExerciseProgressUseCase>();
builder.Services.AddScoped<ICompleteExerciseProgressUseCase, CompleteExerciseProgressUseCase>();

builder.Services.AddScoped<IGetWorkoutByIdUseCase, GetWorkoutByIdUseCase>();

builder.Services.AddScoped<IUpdateWorkoutUseCase, UpdateWorkoutUseCase>();

builder.Services.AddScoped<IDeleteWorkoutUseCase, DeleteWorkoutUseCase>();

builder.Services.AddScoped<IRescheduleWorkoutUseCase, RescheduleWorkoutUseCase>();

builder.Services.AddScoped<IDeleteScheduledWorkoutUseCase, DeleteScheduledWorkoutUseCase>();

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
