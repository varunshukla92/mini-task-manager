using AuthService.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public PasswordHasherService()
        {
            _passwordHasher = new PasswordHasher<User>();
        }
        public string HashPassword(string password)
        {
            var user = new User();
            return _passwordHasher.HashPassword(user, password);
            
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var user = new User();
            var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);

            return result == PasswordVerificationResult.Success;
        }
    }
}
