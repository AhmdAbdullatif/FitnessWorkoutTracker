using Application.Features.Authentication;
using Application.Features.Authentication.Signup;
using FastEndpoints;
using PublicApi.Constants;

namespace PublicApi.Endpoints.Authentication.Signup;

public class SignupEndpoint(ISignupUseCase signupUseCase) : Endpoint<SignupRequest, AuthenticateResponse>
{
    public override void Configure()
    {
        Post("api/auth/signup");
        AllowAnonymous();

        Description(b =>
        {
            b.WithSummary("Register a new user.");
            b.WithDescription("Create a new user account and return an authentication token.");

            b.Produces<AuthenticateResponse>(StatusCodes.Status200OK);
            b.Produces(StatusCodes.Status400BadRequest);
            b.Produces(StatusCodes.Status409Conflict);

            b.WithTags(Constants.Tags.AuthenticationTag);
        });
    }

    public async override Task HandleAsync(SignupRequest req, CancellationToken ct)
    {
        var response = await signupUseCase.ExecuteAsync(
            new SignupCommand(req.Username, req.Email, req.Password));

        await Send.OkAsync(response, cancellation: ct);
    }
}
