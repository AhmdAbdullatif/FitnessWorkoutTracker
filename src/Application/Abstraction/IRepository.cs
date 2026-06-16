using Ardalis.Specification;

namespace Application.Abstraction;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
}
