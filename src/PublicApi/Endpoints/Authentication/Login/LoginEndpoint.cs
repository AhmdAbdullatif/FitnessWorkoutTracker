using Application.Features.Authentication;
using Application.Features.Authentication.Login;
using FastEndpoints;

namespace PublicApi.Endpoints.Authentication.Login
{
    public class LoginEndpoint(ILoginUseCase loginUseCase) : Endpoint<LoginCommand, AuthenticateResponse>
    {
        public override void Configure()
        {
            Post("api/auth/login");
            AllowAnonymous();

            Description(b =>
            {
                b.WithSummary("Login a user.");
                b.WithDescription("Authenticate a user and return a JWT token.");

                b.Produces<AuthenticateResponse>(StatusCodes.Status200OK);
                b.Produces(StatusCodes.Status400BadRequest);

                b.WithTags(Constants.Tags.AuthenticationTag);
            });
        }

        public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
        {
            var response = await loginUseCase.ExecuteAsync(req);

            await Send.OkAsync(response, cancellation: ct);
        }
    }
}
