using AuthService.Models;

namespace AuthService.Services
{
    public interface IJwtTokenService
    {
        (string token, DateTime expiresAt) GenerateToken(User user);
    }
}
