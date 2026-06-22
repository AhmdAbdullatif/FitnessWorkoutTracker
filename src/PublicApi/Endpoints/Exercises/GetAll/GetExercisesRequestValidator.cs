using Application.Features.Exercises.GetAll;
using FastEndpoints;
using FluentValidation;

namespace PublicApi.Endpoints.Exercises.GetAll;

public class GetExercisesRequestValidator : Validator<GetExercisesEndpointRequest>
{
    public GetExercisesRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.");
    }
}
