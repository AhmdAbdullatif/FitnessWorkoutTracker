using Application.Abstraction;
using Ardalis.Specification.EntityFrameworkCore;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class EfRepository<T> : RepositoryBase<T>, IRepository<T>, IReadRepository<T> where T : class
{
    public EfRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

}
