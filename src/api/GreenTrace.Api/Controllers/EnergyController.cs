using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Energy;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
[Authorize]
public class EnergyController : ControllerBase
{
    private readonly IEnergyService _energy;
    public EnergyController(IEnergyService energy) => _energy = energy;

    [HttpGet]
    public async Task<IActionResult> Get(Guid companyId)
    {
        var entries = await _energy.GetEntriesAsync(companyId);
        return Ok(entries.Select(e => e.ToViewModel()));
    }

    [HttpPost]
    public async Task<IActionResult> Add(Guid companyId, CreateEnergyEntryViewModel entry)
    {
        var created = await _energy.AddEntryAsync(companyId, entry.ToEntity(companyId));
        return Ok(created.ToViewModel());
    }
}
