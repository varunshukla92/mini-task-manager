using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth/test")]
    [ApiVersion("1.0")]
    public class AuthTestController : ControllerBase
    {
        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
          
           return Ok(
               new
               {
                   Message = "JWT is valid",
                   User = User.Identity?.Name,
                   Claims = User.Claims.Select(c => new { c.Type, c.Value})
               });
        }
    }
}
