namespace Application.Features.Authentication.Signup;

public interface ISignupUseCase
{
    Task<AuthenticateResponse> ExecuteAsync(SignupCommand command);
}
