using Domain.Entities;

namespace Application.Abstraction;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddUserAsync(User user);
}
