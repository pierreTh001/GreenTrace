using System.Security.Claims;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController(ISubscriptionService subscriptions) : ControllerBase
{
    private readonly ISubscriptionService _subs = subscriptions;

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
        var sub = await _subs.GetActiveAsync(userId);
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
        return Ok(sub);
    }
}

