using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Energy;
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
public class EnergyController : ControllerBase
{
    private readonly IEnergyService _energy;
    private readonly AppDbContext _db;
    public EnergyController(IEnergyService energy, AppDbContext db)
    {
        _energy = energy;
        _db = db;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Liste les entrées d’énergie",
        Description = "Retourne les consommations d’énergie pour l’entreprise indiquée.")]
    public async Task<IActionResult> Get(Guid companyId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var entries = await _energy.GetEntriesAsync(companyId);
        return Ok(entries.Select(e => e.ToViewModel()));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Ajoute une entrée d’énergie",
        Description = "Crée une nouvelle entrée d’énergie pour l’entreprise.")]
    public async Task<IActionResult> Add(Guid companyId, CreateEnergyEntryViewModel entry)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var created = await _energy.AddEntryAsync(companyId, entry.ToEntity(companyId));
        return Ok(created.ToViewModel());
    }
}
