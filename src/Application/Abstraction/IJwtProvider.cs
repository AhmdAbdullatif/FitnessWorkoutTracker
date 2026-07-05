using Domain.Entities;

namespace Application.Abstraction;

public interface IJwtProvider
{
    string CreateAccessToken(Guid id, string email);
    string CreateRefreshToken();
}
