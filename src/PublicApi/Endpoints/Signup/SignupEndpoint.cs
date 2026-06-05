using Application.Features.Authentication.Signup;
using FastEndpoints;

namespace PublicApi.Endpoints.Signup;

public class SignupEndpoint(SignupUseCase signupUseCase) : Endpoint<SignupRequest, SignupResponse>
{
    public override void Configure()
    {
        Post("api/auth/signup");
        AllowAnonymous();
    }

    public async override Task HandleAsync(SignupRequest req, CancellationToken ct)
    {
        var result = await signupUseCase.ExecuteAsync(
            new SignupCommand(req.Username, req.Email, req.Password));

        await SendAsync(new SignupResponse()
        {
            Token = result.Token
        });
    }
}
