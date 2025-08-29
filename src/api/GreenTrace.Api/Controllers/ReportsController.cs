using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Reports;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using GreenTrace.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
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
        Description = "Retourne les rapports de l’entreprise.")]
    public async Task<IActionResult> GetAll(Guid companyId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var result = await _db.Reports.Where(r => r.CompanyId == companyId).ToListAsync();
        return Ok(result.Select(r => r.ToViewModel()));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Détails d’un rapport",
        Description = "Retourne le rapport correspondant à l’identifiant fourni.")]
    public async Task<IActionResult> Get(Guid companyId, Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var report = await _reports.GetByIdAsync(id);
        if (report == null) return NotFound();
        if (report.CompanyId != companyId) return Forbid();
        return Ok(report.ToViewModel());
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Crée un rapport",
        Description = "Crée un nouveau rapport et retourne sa représentation.")]
    public async Task<IActionResult> Create(Guid companyId, CreateReportViewModel report)
    {
        // Check user has role for the company
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        // Force the companyId from route
        report = report with { CompanyId = companyId };
        var created = await _reports.CreateAsync(report.ToEntity());
        return Ok(created.ToViewModel());
    }

    [HttpPost("{id}/lock")]
    [SwaggerOperation(Summary = "Verrouille un rapport",
        Description = "Verrouille le rapport pour empêcher les modifications.")]
    public async Task<IActionResult> Lock(Guid companyId, Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        await _reports.LockAsync(id);
        return Ok();
    }

    [HttpPost("{id}/unlock")]
    [SwaggerOperation(Summary = "Déverrouille un rapport",
        Description = "Rend le rapport à nouveau modifiable.")]
    public async Task<IActionResult> Unlock(Guid companyId, Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        await _reports.UnlockAsync(id);
        return Ok();
    }

    [HttpPost("{id}/sign-off")]
    [SwaggerOperation(Summary = "Valide un rapport",
        Description = "Marque le rapport comme approuvé/signé.")]
    public async Task<IActionResult> SignOff(Guid companyId, Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        await _reports.SignOffAsync(id);
        return Ok();
    }
}
