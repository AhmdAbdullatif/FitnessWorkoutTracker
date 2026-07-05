using Ardalis.Specification;
using Domain.Entities;

namespace Application.Specifications;

public class GetRefreshTokenSpec : Specification<RefreshToken>
{
    public GetRefreshTokenSpec(string token)
    {
        Query
            .Include(x => x.User)
            .Where(x => x.Token == token);
    }
}
