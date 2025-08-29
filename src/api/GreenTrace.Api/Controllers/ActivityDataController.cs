using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.ActivityData;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using GreenTrace.Api.Infrastructure;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class ActivityDataController : ControllerBase
{
    private readonly IActivityDataService _activity;
    private readonly AppDbContext _db;
    public ActivityDataController(IActivityDataService activity, AppDbContext db)
    {
        _activity = activity;
        _db = db;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Ingestion d’une activité",
        Description = "Crée une entrée d’activité (période, quantité, facteur) pour la société.")]
    public async Task<IActionResult> Ingest(ActivityDataViewModel data)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == data.CompanyId);
        if (!allowed) return Forbid();
        await _activity.IngestAsync(data.ToEntity());
        return Ok();
    }
}
