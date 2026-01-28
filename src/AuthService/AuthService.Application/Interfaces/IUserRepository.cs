using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    }
}
