using AuthService.Application.Interfaces;
using AuthService.Application.Options;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public (string token, DateTime expiresAt) GenerateToken(User user)
        {

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("User email is missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Aud, _options.Audience)
            };

            var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
             );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
