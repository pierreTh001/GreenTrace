using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmissionFactorsController : ControllerBase
{
    private readonly IEmissionFactorService _factors;
    public EmissionFactorsController(IEmissionFactorService factors) => _factors = factors;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var list = await _factors.ListAsync();
        return Ok(list);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var result = await _factors.SearchAsync(q);
        return Ok(result);
    }
}
