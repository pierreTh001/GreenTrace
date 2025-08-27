using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.ActivityData;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivityDataController : ControllerBase
{
    private readonly IActivityDataService _activity;
    public ActivityDataController(IActivityDataService activity) => _activity = activity;

    [HttpPost]
    public async Task<IActionResult> Ingest(ActivityDataViewModel data)
    {
        await _activity.IngestAsync(data.ToEntity());
        return Ok();
    }
}
