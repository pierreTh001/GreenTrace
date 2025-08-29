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
public class EmissionsController : ControllerBase
{
    private readonly IEmissionService _emissions;
    private readonly AppDbContext _db;
    public EmissionsController(IEmissionService emissions, AppDbContext db)
    {
        _emissions = emissions;
        _db = db;
    }

    [HttpGet("{reportId}")]
    [SwaggerOperation(Summary = "Calcule les émissions d’un rapport",
        Description = "Calcule et retourne les émissions (tCO2e) pour le rapport fourni.")]
    public async Task<IActionResult> Calculate(Guid reportId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var report = await _db.Reports.FindAsync(reportId);
        if (report == null) return NotFound();
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == report.CompanyId);
        if (!allowed) return Forbid();
        var result = await _emissions.CalculateAsync(reportId);
        return Ok(result);
    }
}
