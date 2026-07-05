namespace Application.Features.Authentication.Login;

public interface ILoginUseCase
{
    Task<AuthenticateResponse> ExecuteAsync(LoginCommand command);
}
