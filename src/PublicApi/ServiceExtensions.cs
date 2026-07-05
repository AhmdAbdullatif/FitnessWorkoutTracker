using System.Text;
using Application.Abstraction;
using Application.Features.Authentication.Login;
using Application.Features.Authentication.Refresh;
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
using Application.Features.ScheduledWorkouts.GetAll;
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
using Infrastructure.Logging;
using Infrastructure.Services;
using Infrastructure.Services.Authentication;
using Infrastructure.Services.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PublicApi.Services;

namespace PublicApi;

public static class ServiceExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddAuthentication()
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

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        services.AddScoped<ISignupUseCase, SignupUseCase>();
        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<ICreateWorkoutUseCase, CreateWorkoutUseCase>();
        services.AddScoped<IGetWorkoutsUseCase, GetWorkoutsUseCase>();

        services.AddScoped<ICreateExerciseUseCase, CreateExerciseUseCase>();

        services.AddScoped<IGetExercisesUseCase, GetExercisesUseCase>();

        services.AddSingleton<IUtcLocalConverter, UtcLocalConverter>();

        services.AddScoped<IScheduleWorkoutUseCase, ScheduleWorkoutUseCase>();

        services.AddScoped<IStartScheduledWorkoutUseCase, StartScheduledWorkoutUseCase>();

        services.AddScoped<IStartExerciseProgressUseCase, StartExerciseProgressUseCase>();
        services.AddScoped<IDeleteExerciseProgressUseCase, DeleteExerciseProgressUseCase>();

        services.AddScoped<IGetExerciseProgressesUseCase, GetExerciseProgressesUseCase>();

        services.AddScoped<IAddNoteToExerciseProgressUseCase, AddNoteToExerciseProgressUseCase>();

        services.AddScoped<IGetExerciseProgressByIdUseCase, GetExerciseProgressByIdUseCase>();

        services.AddScoped<IGetScheduledWorkoutByIdUseCase, GetScheduledWorkoutByIdUseCase>();

        services.AddScoped<IFinishScheduledWorkoutUseCase, FinishScheduledWorkoutUseCase>();

        services.AddScoped<ICancelScheduledWorkoutUseCase, CancelScheduledWorkoutUseCase>();

        services.AddScoped<ISkipExerciseProgressUseCase, SkipExerciseProgressUseCase>();
        services.AddScoped<ICompleteExerciseProgressUseCase, CompleteExerciseProgressUseCase>();

        services.AddScoped<IGetWorkoutByIdUseCase, GetWorkoutByIdUseCase>();

        services.AddScoped<IUpdateWorkoutUseCase, UpdateWorkoutUseCase>();

        services.AddScoped<IDeleteWorkoutUseCase, DeleteWorkoutUseCase>();

        services.AddScoped<IRescheduleWorkoutUseCase, RescheduleWorkoutUseCase>();

        services.AddScoped<IDeleteScheduledWorkoutUseCase, DeleteScheduledWorkoutUseCase>();

        services.AddScoped<IGetScheduledWorkoutsUseCase, GetScheduledWorkoutsUseCase>();

        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();

        return services;
    }
}
