using Ardalis.Specification;

namespace Application.Abstraction;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
{
}
