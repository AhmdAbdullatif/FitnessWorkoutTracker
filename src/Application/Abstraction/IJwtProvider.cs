using Domain.Entities;

namespace Application.Abstraction;

public interface IJwtProvider
{
    string Create(Guid id, string email);
}
