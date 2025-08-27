using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.IdentityModel.Tokens;

namespace GreenTrace.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _users;
    private readonly IConfiguration _config;

    public AuthService(IUserService users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var result = await _users.LoginAsync(email, password);
        if (result == null) return null;
        return GenerateToken(result.Value.user, result.Value.roles);
    }

    public Task<string> RefreshTokenAsync(string token)
        => Task.FromResult(token);

    public Task LogoutAsync() => Task.CompletedTask;

    public Task VerifyEmailAsync(string token) => Task.CompletedTask;

    public Task ResetPasswordAsync(string email) => Task.CompletedTask;

    private string GenerateToken(User user, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
}
