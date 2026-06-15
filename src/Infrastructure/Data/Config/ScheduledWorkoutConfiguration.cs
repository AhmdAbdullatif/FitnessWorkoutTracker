using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace Infrastructure.Data.Config
{
    internal class ScheduledWorkoutConfiguration : IEntityTypeConfiguration<ScheduledWorkout>
    {
        public void Configure(EntityTypeBuilder<ScheduledWorkout> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Status)
                .HasConversion<string>();

            var instantConverter = new ValueConverter<Instant, DateTime>(
               v => v.ToDateTimeUtc(),
               v => Instant.FromDateTimeUtc(DateTime.SpecifyKind(v, DateTimeKind.Utc))
           );

            builder.Property(x => x.SessionDate)
                .HasConversion(instantConverter)
                .HasColumnType("datetime2");

            builder.Property(x => x.StartedAt)
                .HasConversion(instantConverter)
                .HasColumnType("datetime2");

            builder.Property(x => x.CompletedAt)
                .HasConversion(instantConverter)
                .HasColumnType("datetime2");

            builder.HasOne(x => x.Workout)
                .WithMany(u => u.ScheduledWorkouts)
                .HasForeignKey(x => x.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
