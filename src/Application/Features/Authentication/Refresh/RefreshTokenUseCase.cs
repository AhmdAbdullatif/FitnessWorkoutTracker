using Application.Abstraction;
using Application.Exceptions;
using Application.Specifications;
using Domain.Entities;

namespace Application.Features.Authentication.Refresh;

public class RefreshTokenUseCase(IRepository<RefreshToken> repository,
    IJwtProvider jwtProvider,
    IAppLogger<RefreshTokenUseCase> logger) : IRefreshTokenUseCase
{
    public async Task<AuthenticateResponse> ExecuteAsync(string refreshToken)
    {
        var spec = new GetRefreshTokenSpec(refreshToken);
        var oldRefreshToken = await repository.FirstOrDefaultAsync(spec);

        if (oldRefreshToken is null || oldRefreshToken.ExpiresUtc < DateTime.UtcNow)
            throw new UnauthorizedAccessException();
        

        if (oldRefreshToken.RevokedUtc != null)
        {
            logger.LogWarning("User tried to use a revoked refresh token: {RefreshToken}." +
            " Consider invalidating all their tokens and forcing them to login.", oldRefreshToken.Token);
            throw new UnauthorizedAccessException();
        }

        var userId = oldRefreshToken.User!.Id;
        var newRefreshToken = new RefreshToken(jwtProvider.CreateRefreshToken(), userId);
        oldRefreshToken.Revoke();

        await repository.AddAsync(newRefreshToken);

        var accessToken = jwtProvider.CreateAccessToken(userId, oldRefreshToken.User!.Email);

        return new AuthenticateResponse()
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token
        };
    }
}
