using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;

namespace AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var email = request.Email.ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(email,ct);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var isValid = _passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash!
            );

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                Role = user.Role,
                AccessToken = token,
                ExpiresAt = expiresAt,
                Message = "Login successful"
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            var email = request.Email.ToLowerInvariant();

            var exists = await _userRepository.ExistsByEmailAsync(email, ct);

            if (exists)
            {
                throw new InvalidOperationException("Email already registered");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = "User",
                IsActive = true
            };

            await _userRepository.AddAsync(user, ct);

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                Role = user.Role,
                Message = "User Registered Successfully"
            };
        }
    }


}
