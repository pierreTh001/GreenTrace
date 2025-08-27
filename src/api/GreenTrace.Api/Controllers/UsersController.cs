using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    private readonly IConfiguration _config;

    public UsersController(IUserService users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        try
        {
            var result = await _users.RegisterAsync(req.Email, req.Password, req.FirstName, req.LastName);
            var token = GenerateToken(result.user, result.roles);
            return Ok(new { token });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var result = await _users.LoginAsync(req.Email, req.Password);
        if (result == null)
        {
            return Unauthorized();
        }

        var token = GenerateToken(result.Value.user, result.Value.roles);
        return Ok(new { token });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // For JWT, logout is handled on the client side.
        return Ok();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _users.GetAllAsync();
        var result = users.Select(u => new { u.Id, u.Email, u.FirstName, u.LastName });
        return Ok(result);
    }

    private string GenerateToken(User user, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var r in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiryMinutes")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
    public record LoginRequest(string Email, string Password);
}
