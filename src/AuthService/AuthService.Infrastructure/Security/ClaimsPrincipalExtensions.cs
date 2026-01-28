using System.Security.Claims;

namespace AuthService.Infrastructure.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if( user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if(userIdClaim == null)
                throw new UnauthorizedAccessException("UserId claim not found");

            return Guid.Parse(userIdClaim.Value);
        }
    }
}
