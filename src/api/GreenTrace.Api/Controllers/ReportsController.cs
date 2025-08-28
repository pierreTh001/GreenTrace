using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Reports;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using GreenTrace.Api.Infrastructure;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    private readonly AppDbContext _db;
    public ReportsController(IReportService reports, AppDbContext db)
    {
        _reports = reports;
        _db = db;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Liste des rapports",
        Description = "Retourne tous les rapports disponibles.")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reports.GetAllAsync();
        return Ok(result.Select(r => r.ToViewModel()));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Détails d’un rapport",
        Description = "Retourne le rapport correspondant à l’identifiant fourni.")]
    public async Task<IActionResult> Get(Guid id)
    {
        var report = await _reports.GetByIdAsync(id);
        if (report == null) return NotFound();
        return Ok(report.ToViewModel());
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Crée un rapport",
        Description = "Crée un nouveau rapport et retourne sa représentation.")]
    public async Task<IActionResult> Create(CreateReportViewModel report)
    {
        // Check user has role for the company
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == report.CompanyId);
        if (!allowed) return Forbid();
        var created = await _reports.CreateAsync(report.ToEntity());
        return Ok(created.ToViewModel());
    }

    [HttpPost("{id}/lock")]
    [SwaggerOperation(Summary = "Verrouille un rapport",
        Description = "Verrouille le rapport pour empêcher les modifications.")]
    public async Task<IActionResult> Lock(Guid id)
    {
        await _reports.LockAsync(id);
        return Ok();
    }

    [HttpPost("{id}/unlock")]
    [SwaggerOperation(Summary = "Déverrouille un rapport",
        Description = "Rend le rapport à nouveau modifiable.")]
    public async Task<IActionResult> Unlock(Guid id)
    {
        await _reports.UnlockAsync(id);
        return Ok();
    }

    [HttpPost("{id}/sign-off")]
    [SwaggerOperation(Summary = "Valide un rapport",
        Description = "Marque le rapport comme approuvé/signé.")]
    public async Task<IActionResult> SignOff(Guid id)
    {
        await _reports.SignOffAsync(id);
        return Ok();
    }
}
