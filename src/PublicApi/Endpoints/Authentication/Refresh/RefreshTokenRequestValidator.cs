using FastEndpoints;
using FluentValidation;

namespace PublicApi.Endpoints.Authentication.Refresh;

public class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotNull().NotEmpty()
            .WithMessage("Provide valid refresh token.");
    }
}
