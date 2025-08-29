using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Sites;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using GreenTrace.Api.Infrastructure;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
[Authorize(Policy = "Subscribed")]
public class SitesController : ControllerBase
{
    private readonly ISiteService _sites;
    private readonly AppDbContext _db;
    public SitesController(ISiteService sites, AppDbContext db)
    {
        _sites = sites;
        _db = db;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Liste des sites d’une entreprise",
        Description = "Retourne les sites rattachés à l’entreprise indiquée.")]
    public async Task<IActionResult> Get(Guid companyId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var sites = await _sites.GetByCompanyAsync(companyId);
        return Ok(sites.Select(s => s.ToViewModel()));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Crée un site",
        Description = "Ajoute un site à l’entreprise.")]
    public async Task<IActionResult> Create(Guid companyId, CreateSiteViewModel site)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var created = await _sites.CreateAsync(companyId, site.ToEntity(companyId));
        return Ok(created.ToViewModel());
    }
}
