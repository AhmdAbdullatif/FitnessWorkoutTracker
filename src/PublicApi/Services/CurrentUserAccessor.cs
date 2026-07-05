using Application.Abstraction;
using System.Security.Claims;

namespace PublicApi.Services
{
    public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
    {
        public string GetEmail()
        {
            return httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!.Value;
        }

        public Guid GetId()
        {
            return Guid.Parse(httpContextAccessor.HttpContext!.User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
