using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace Infrastructure.Data.Config
{
    internal class ExerciseProgressConfiguration : IEntityTypeConfiguration<ExerciseProgress>
    {
        public void Configure(EntityTypeBuilder<ExerciseProgress> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Status)
                .HasConversion<string>();

            var instantConverter = new ValueConverter<Instant, DateTime>(
                v => v.ToDateTimeUtc(),
                v => Instant.FromDateTimeUtc(DateTime.SpecifyKind(v, DateTimeKind.Utc))
            );

            builder.Property(x => x.StartedAt)
                .HasConversion(instantConverter)
                .HasColumnType("datetime2");
            
            builder.Property(x => x.CompletedAt)
                .HasConversion(instantConverter)
                .HasColumnType("datetime2");

            builder.HasOne(x => x.Exercise)
                .WithMany(u => u.ExerciseProgresses)
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ScheduledWorkout)
                .WithMany(u => u.ExerciseProgresses)
                .HasForeignKey(x => x.ScheduledWorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Notes)
                .WithOne()
                .HasForeignKey(x => x.ExerciseProgressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
