using AuthService.Application.DTOs;


namespace AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);

        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    }
}
