using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
    Task<string> RefreshTokenAsync(string token);
    Task LogoutAsync();
    Task VerifyEmailAsync(string token);
    Task ResetPasswordAsync(string email);
}
