using Application.Features.Authentication;
using Application.Features.Authentication.Refresh;
using FastEndpoints;

namespace PublicApi.Endpoints.Authentication.Refresh;

public class RefreshTokenEndpoint(IRefreshTokenUseCase refreshTokenUseCase)
    : Endpoint<RefreshTokenRequest, AuthenticateResponse>
{
    public override void Configure()
    {
        Post("api/auth/refresh-token");
        AllowAnonymous();
        
        Description(b =>
        {
            b.WithSummary("Refresh user access token.");
            b.WithDescription("Get a new access token by using a refresh token.");

            b.Produces<AuthenticateResponse>(StatusCodes.Status200OK);
            b.Produces(StatusCodes.Status401Unauthorized);

            b.WithTags(Constants.Tags.AuthenticationTag);
        });
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        AuthenticateResponse response;
        try
        {
            response = await refreshTokenUseCase.ExecuteAsync(req.RefreshToken);
        } catch (UnauthorizedAccessException)
        {
            await Send.UnauthorizedAsync(cancellation: ct);
            return;
        }

        await Send.OkAsync(response, cancellation: ct);
    }
}
