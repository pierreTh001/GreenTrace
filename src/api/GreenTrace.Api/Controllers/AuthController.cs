using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var token = await _auth.LoginAsync(req.Email, req.Password);
        if (token == null) return Unauthorized();
        return Ok(new { token });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(TokenRequest req)
    {
        var token = await _auth.RefreshTokenAsync(req.Token);
        return Ok(new { token });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _auth.LogoutAsync();
        return Ok();
    }

    [HttpGet("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        await _auth.VerifyEmailAsync(token);
        return Ok();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
    {
        await _auth.ResetPasswordAsync(req.Email);
        return Ok();
    }

    public record LoginRequest(string Email, string Password);
    public record TokenRequest(string Token);
    public record ResetPasswordRequest(string Email);
}
