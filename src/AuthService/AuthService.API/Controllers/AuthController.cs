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

        public AuthController(
           IAuthService authService)
        {
            _authService = authService;
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
                var response = await _authService.RegisterAsync(request,cancellationToken);
                return Ok(response);
            }
            catch(InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.LoginAsync(request, cancellationToken);
                return Ok(response);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
