using IdentityService.Api.Application.Services;
using System.Security.Claims;

namespace BasketService.Api.Core.Application.Services
{
    public class IdentityServices : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
