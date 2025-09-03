using System.Security.Claims;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController(ISubscriptionService subscriptions, IUserService users) : ControllerBase
{
    private readonly ISubscriptionService _subs = subscriptions;
    private readonly IUserService _users = users;

    [HttpGet("plans")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Liste les offres d’abonnement", Description = "Retourne les plans disponibles (Basic, Pro).")]
    public async Task<IActionResult> Plans()
        => Ok(await _subs.GetPlansAsync());

    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "État de l’abonnement courant", Description = "Retourne l’abonnement actif de l’utilisateur.")]
    public async Task<IActionResult> Me()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var sub = await _subs.GetCurrentAsync(userId);
        return Ok(sub);
    }

    public record SubscribeRequest(Guid PlanId);

    [HttpPost("subscribe")]
    [Authorize]
    [SwaggerOperation(Summary = "Souscrit à une offre", Description = "Active un abonnement pour l’utilisateur connecté (paiement simulé).")]
    public async Task<IActionResult> Subscribe(SubscribeRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var sub = await _subs.SubscribeAsync(userId, req.PlanId);
        // Reactivate account if it was scheduled for deletion
        await _users.ReactivateAsync(userId);
        return Ok(sub);
    }

    [HttpPost("cancel")]
    [Authorize]
    [SwaggerOperation(Summary = "Annule le renouvellement", Description = "Met fin à l'abonnement à la date anniversaire (pas de renouvellement).")]
    public async Task<IActionResult> Cancel()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var sub = await _subs.CancelAsync(userId);
        return Ok(sub);
    }
}
