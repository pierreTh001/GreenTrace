using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.Services;
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
        return Ok(entries);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Guid companyId, EnergyEntry entry)
    {
        var created = await _energy.AddEntryAsync(companyId, entry);
        return Ok(created);
    }
}
