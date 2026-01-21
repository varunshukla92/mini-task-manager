using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class AuthApplicationService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthApplicationService(
            AuthDbContext context,
            IPasswordHasherService passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.ToLowerInvariant();

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var isValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash!);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid Credentials");
            }

            var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

            return new AuthResponse
            {
                UserId= user.Id,
                Email = user.Email!,
                Role= user.Role,
                AccessToken = token,
                ExpiresAt = expiresAt,
                Message = "Login successful"
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.ToLowerInvariant();

            var exists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (exists)
            {
                throw new InvalidOperationException("Email already registered");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                UserId = user.Id,
                Email = email,
                Role = user.Role,
                Message = "User Registered Successfully"
            };
        }
    }
}