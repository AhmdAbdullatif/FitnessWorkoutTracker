using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehaviorOnEntityRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseProgresses_Exercises_ExerciseId",
                table: "ExerciseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseProgresses_ScheduledWorkouts_ScheduledWorkoutId",
                table: "ExerciseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_ExerciseProgresses_ExerciseProgressId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledWorkouts_Workouts_WorkoutId",
                table: "ScheduledWorkouts");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseProgresses_Exercises_ExerciseId",
                table: "ExerciseProgresses",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseProgresses_ScheduledWorkouts_ScheduledWorkoutId",
                table: "ExerciseProgresses",
                column: "ScheduledWorkoutId",
                principalTable: "ScheduledWorkouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_ExerciseProgresses_ExerciseProgressId",
                table: "Notes",
                column: "ExerciseProgressId",
                principalTable: "ExerciseProgresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledWorkouts_Workouts_WorkoutId",
                table: "ScheduledWorkouts",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseProgresses_Exercises_ExerciseId",
                table: "ExerciseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseProgresses_ScheduledWorkouts_ScheduledWorkoutId",
                table: "ExerciseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_ExerciseProgresses_ExerciseProgressId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledWorkouts_Workouts_WorkoutId",
                table: "ScheduledWorkouts");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseProgresses_Exercises_ExerciseId",
                table: "ExerciseProgresses",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseProgresses_ScheduledWorkouts_ScheduledWorkoutId",
                table: "ExerciseProgresses",
                column: "ScheduledWorkoutId",
                principalTable: "ScheduledWorkouts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_ExerciseProgresses_ExerciseProgressId",
                table: "Notes",
                column: "ExerciseProgressId",
                principalTable: "ExerciseProgresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledWorkouts_Workouts_WorkoutId",
                table: "ScheduledWorkouts",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id");
        }
    }
}
