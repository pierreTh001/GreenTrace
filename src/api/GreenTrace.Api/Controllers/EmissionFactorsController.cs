using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class EmissionFactorsController : ControllerBase
{
    private readonly IEmissionFactorService _factors;
    public EmissionFactorsController(IEmissionFactorService factors) => _factors = factors;

    [HttpGet]
    [SwaggerOperation(Summary = "Liste des facteurs d’émission",
        Description = "Retourne tous les facteurs d’émission disponibles.")]
    public async Task<IActionResult> List()
    {
        var list = await _factors.ListAsync();
        return Ok(list);
    }

    [HttpGet("search")]
    [SwaggerOperation(Summary = "Recherche de facteurs d’émission",
        Description = "Recherche des facteurs via un mot-clé passé en paramètre.")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var result = await _factors.SearchAsync(q);
        return Ok(result);
    }
}
