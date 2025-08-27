using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmissionsController : ControllerBase
{
    private readonly IEmissionService _emissions;
    public EmissionsController(IEmissionService emissions) => _emissions = emissions;

    [HttpGet("{reportId}")]
    public async Task<IActionResult> Calculate(Guid reportId)
    {
        var result = await _emissions.CalculateAsync(reportId);
        return Ok(result);
    }
}
