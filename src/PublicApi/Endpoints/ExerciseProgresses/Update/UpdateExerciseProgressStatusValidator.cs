using System.ComponentModel.DataAnnotations;
using Application.Features.ExerciseProgresses.Update;
using FastEndpoints;
using FluentValidation;

namespace PublicApi.Endpoints.ExerciseProgresses.Update;

public class UpdateExerciseProgressStatusValidator : Validator<UpdateExerciseProgressStatusRequest>
{
    public UpdateExerciseProgressStatusValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
