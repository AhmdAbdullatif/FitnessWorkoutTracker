using Application.Features.Workouts.GetAll;
using FastEndpoints;
using FluentValidation;

namespace PublicApi.Endpoints.Workouts.GetAll;

public class GetWorkoutsRequestValidator : Validator<GetWorkoutsEndpointRequest>
{
    public GetWorkoutsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.");
    }
}
