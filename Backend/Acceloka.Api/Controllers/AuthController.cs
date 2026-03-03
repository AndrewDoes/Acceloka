using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Acceloka.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            // hardcode for next.js app
            var redirectUrl = "http://localhost:3000";

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            // trigger the Google login screen
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    IsAuthenticated = true,
                    Email = User.FindFirst(ClaimTypes.Email)?.Value,
                    Name = User.FindFirst(ClaimTypes.Name)?.Value,
                    InternalUserId = User.FindFirst("InternalUserId")?.Value,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value
                });
            }

            return Ok(new { IsAuthenticated = false });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Logged out successfully" });
        }
    }
}