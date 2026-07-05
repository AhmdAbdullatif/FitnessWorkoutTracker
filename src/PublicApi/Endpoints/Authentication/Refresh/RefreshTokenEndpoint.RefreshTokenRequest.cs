using Microsoft.AspNetCore.Mvc;

namespace PublicApi.Endpoints.Authentication.Refresh;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}