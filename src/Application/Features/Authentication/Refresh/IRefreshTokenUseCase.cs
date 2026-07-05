namespace Application.Features.Authentication.Refresh;

public interface IRefreshTokenUseCase
{
    Task<AuthenticateResponse> ExecuteAsync(string refreshToken);
}
