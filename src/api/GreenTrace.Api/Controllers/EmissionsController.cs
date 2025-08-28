using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class EmissionsController : ControllerBase
{
    private readonly IEmissionService _emissions;
    public EmissionsController(IEmissionService emissions) => _emissions = emissions;

    [HttpGet("{reportId}")]
    [SwaggerOperation(Summary = "Calcule les émissions d’un rapport",
        Description = "Calcule et retourne les émissions (tCO2e) pour le rapport fourni.")]
    public async Task<IActionResult> Calculate(Guid reportId)
    {
        var result = await _emissions.CalculateAsync(reportId);
        return Ok(result);
    }
}
