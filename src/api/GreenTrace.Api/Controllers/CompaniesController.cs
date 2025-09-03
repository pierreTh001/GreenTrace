using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Companies;
using GreenTrace.Api.Infrastructure.Entities;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using GreenTrace.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companies;
    private readonly AppDbContext _db;
    private readonly ICompanyLookupService _lookup;
    public CompaniesController(ICompanyService companies, AppDbContext db, ICompanyLookupService lookup)
    {
        _companies = companies;
        _db = db;
        _lookup = lookup;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Liste des entreprises",
        Description = "Retourne les entreprises auxquelles l’utilisateur appartient.")]
    public async Task<IActionResult> GetAll()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var companyIds = _db.UserCompanyRoles.Where(u => u.UserId == userId).Select(u => u.CompanyId).Distinct();
        var result = await _db.Companies.Where(c => companyIds.Contains(c.Id)).ToListAsync();
        return Ok(result.Select(c => c.ToViewModel()));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Détails d’une entreprise",
        Description = "Retourne l’entreprise correspondant à l’identifiant fourni.")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == id);
        if (!allowed) return Forbid();
        var company = await _companies.GetByIdAsync(id);
        if (company == null) return NotFound();
        return Ok(company.ToViewModel());
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation(Summary = "Crée une entreprise",
        Description = "Crée une nouvelle entreprise et retourne sa représentation.")]
    public async Task<IActionResult> Create(CreateCompanyViewModel company)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var ownerId = Guid.Parse(userIdStr!);
        var created = await _companies.CreateForUserAsync(ownerId, company.ToEntity());
        return Ok(created.ToViewModel());
    }

    [HttpPut("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Met à jour une entreprise",
        Description = "Met à jour les informations de l’entreprise spécifiée.")]
    public async Task<IActionResult> Update(Guid id, UpdateCompanyViewModel company)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == id);
        if (!allowed) return Forbid();
        try
        {
            var entity = new Company();
            company.MapTo(entity);
            var updated = await _companies.UpdateAsync(id, entity);
            return Ok(updated.ToViewModel());
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Supprime une entreprise",
        Description = "Supprime l’entreprise identifiée (suppression logique si applicable).")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == id);
        if (!allowed) return Forbid();
        await _companies.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/parent/{parentId}")]
    [Authorize]
    [SwaggerOperation(Summary = "Définit la société mère",
        Description = "Lie l’entreprise à une société parente.")]
    public async Task<IActionResult> SetParent(Guid id, Guid parentId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == id);
        if (!allowed) return Forbid();
        await _companies.SetParentAsync(id, parentId);
        return Ok();
    }

    [HttpGet("lookup")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Recherche par raison sociale",
        Description = "Retourne des suggestions (raison sociale → SIREN/SIRET/TVA/NACE/formejuridique/adresse).")]
    public async Task<IActionResult> Lookup([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Ok(Array.Empty<object>());
        var result = await _lookup.SearchAsync(query);
        return Ok(result);
    }
}
