using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Companies;
using GreenTrace.Api.Infrastructure.Entities;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companies;
    public CompaniesController(ICompanyService companies) => _companies = companies;

    [HttpGet]
    [SwaggerOperation(Summary = "Liste des entreprises",
        Description = "Retourne la liste de toutes les entreprises.")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _companies.GetAllAsync();
        return Ok(result.Select(c => c.ToViewModel()));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Détails d’une entreprise",
        Description = "Retourne l’entreprise correspondant à l’identifiant fourni.")]
    public async Task<IActionResult> Get(Guid id)
    {
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
        var entity = new Company();
        company.MapTo(entity);
        var updated = await _companies.UpdateAsync(id, entity);
        return Ok(updated.ToViewModel());
    }

    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Supprime une entreprise",
        Description = "Supprime l’entreprise identifiée (suppression logique si applicable).")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _companies.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/parent/{parentId}")]
    [Authorize]
    [SwaggerOperation(Summary = "Définit la société mère",
        Description = "Lie l’entreprise à une société parente.")]
    public async Task<IActionResult> SetParent(Guid id, Guid parentId)
    {
        await _companies.SetParentAsync(id, parentId);
        return Ok();
    }
}
