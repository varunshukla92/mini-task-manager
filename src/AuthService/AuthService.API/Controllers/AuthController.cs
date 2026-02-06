using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Interfaces;
using AuthService.Application.DTOs;

namespace AuthService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
           IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            try
            {
                _logger.LogInformation("Registration started for {Email}", request.Email);
                var response = await _authService.RegisterAsync(request,cancellationToken);
                _logger.LogInformation("Registration successful for {Email}", request.Email);
                return Ok(response);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration conflict for {Email}", request.Email);
                return Conflict(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {            
            try
            {
                _logger.LogInformation("Login attempt for {Email}", request.Email);
                var response = await _authService.LoginAsync(request, cancellationToken);
                _logger.LogInformation("Login successful for {Email}", request.Email);
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Login failed for {Email}", request.Email);
                return Unauthorized(ex.Message);
            }
        }
    }
}
