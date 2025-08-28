using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Energy;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
[Authorize(Policy = "Subscribed")]
public class EnergyController : ControllerBase
{
    private readonly IEnergyService _energy;
    public EnergyController(IEnergyService energy) => _energy = energy;

    [HttpGet]
    [SwaggerOperation(Summary = "Liste les entrées d’énergie",
        Description = "Retourne les consommations d’énergie pour l’entreprise indiquée.")]
    public async Task<IActionResult> Get(Guid companyId)
    {
        var entries = await _energy.GetEntriesAsync(companyId);
        return Ok(entries.Select(e => e.ToViewModel()));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Ajoute une entrée d’énergie",
        Description = "Crée une nouvelle entrée d’énergie pour l’entreprise.")]
    public async Task<IActionResult> Add(Guid companyId, CreateEnergyEntryViewModel entry)
    {
        var created = await _energy.AddEntryAsync(companyId, entry.ToEntity(companyId));
        return Ok(created.ToViewModel());
    }
}
