using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications;

public class GetUserByEmailReadonlySpec : Specification<User>
{
    public GetUserByEmailReadonlySpec(string email)
    {
        Query.AsNoTracking()
            .Where(x => x.Email == email);
    }
}
