using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IUserService _users;

    public AuthController(IAuthService auth, IUserService users)
    {
        _auth = auth;
        _users = users;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Authentification et obtention du JWT",
        Description = "Vérifie l’email et le mot de passe puis renvoie un jeton JWT.")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var token = await _auth.LoginAsync(req.Email, req.Password);
        if (token == null) return Unauthorized();
        return Ok(new { token });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Rafraîchit le jeton",
        Description = "Émet un nouveau JWT à partir d’un jeton valide.")]
    public async Task<IActionResult> Refresh(TokenRequest req)
    {
        var token = await _auth.RefreshTokenAsync(req.Token);
        return Ok(new { token });
    }

    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(Summary = "Déconnexion",
        Description = "Révoque la session de l’utilisateur courant.")]
    public async Task<IActionResult> Logout()
    {
        await _auth.LogoutAsync();
        return Ok();
    }

    [HttpGet("verify-email")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Vérifie l’adresse email",
        Description = "Active le compte via le jeton de vérification transmis en paramètre.")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        await _auth.VerifyEmailAsync(token);
        return Ok();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Demande de réinitialisation du mot de passe",
        Description = "Envoie un email de réinitialisation si le compte existe.")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
    {
        await _auth.ResetPasswordAsync(req.Email);
        return Ok();
    }

    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Crée un compte utilisateur",
        Description = "Inscrit un nouvel utilisateur et renvoie un token JWT.")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var result = await _users.RegisterAsync(req.Email, req.Password, req.FirstName, req.LastName);
        var token = await _auth.LoginAsync(req.Email, req.Password);
        return Ok(new { token });
    }

    public record LoginRequest(string Email, string Password);
    public record TokenRequest(string Token);
    public record ResetPasswordRequest(string Email);
}
