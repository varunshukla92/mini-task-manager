using System;

namespace AuthService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Email { get; set; } = null;
        public string? PasswordHash { get; set; } = null;
        public string Role { get; set; } = "User";
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

    }
}
