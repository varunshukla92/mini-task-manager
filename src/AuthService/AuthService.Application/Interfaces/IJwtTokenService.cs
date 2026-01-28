using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface IJwtTokenService
    {
        (string token, DateTime expiresAt) GenerateToken(User user);
    }
}
