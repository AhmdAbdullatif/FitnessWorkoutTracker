using Application.Features.ScheduledWorkouts.GetAll;
using FastEndpoints;
using FluentValidation;

namespace PublicApi.Endpoints.ScheduledWorkouts.GetAll;

public class GetScheduledWorkoutsRequestValidator : Validator<GetScheduledWorkoutsEndpointRequest>
{
    public GetScheduledWorkoutsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.");
    }
}
