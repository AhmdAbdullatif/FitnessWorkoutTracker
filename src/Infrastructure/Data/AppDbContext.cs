using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Workout> Workouts => Set<Workout>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<ScheduledWorkout> ScheduledWorkouts => Set<ScheduledWorkout>();
        public DbSet<ExerciseProgress> ExerciseProgresses => Set<ExerciseProgress>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
