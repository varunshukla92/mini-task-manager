using System.Security.Claims;

namespace TaskService.API.Extensions
{
    public static class UserClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userId!);
        }
    }
}
