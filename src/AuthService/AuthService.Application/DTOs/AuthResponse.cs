namespace AuthService.Application.DTOs
{
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? AccessToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Message { get; set; } = null!;
    }
}
