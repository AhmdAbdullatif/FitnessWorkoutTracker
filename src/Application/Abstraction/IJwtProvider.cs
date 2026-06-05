using Domain.Entities;

namespace Application.Abstraction;

public interface IJwtProvider
{
    string Create(User user);
}
