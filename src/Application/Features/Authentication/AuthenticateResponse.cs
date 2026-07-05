namespace Application.Features.Authentication;

public class AuthenticateResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
