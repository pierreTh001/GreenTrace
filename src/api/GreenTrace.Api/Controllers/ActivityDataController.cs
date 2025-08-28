using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.ActivityData;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class ActivityDataController : ControllerBase
{
    private readonly IActivityDataService _activity;
    public ActivityDataController(IActivityDataService activity) => _activity = activity;

    [HttpPost]
    [SwaggerOperation(Summary = "Ingestion d’une activité",
        Description = "Crée une entrée d’activité (période, quantité, facteur) pour la société.")]
    public async Task<IActionResult> Ingest(ActivityDataViewModel data)
    {
        await _activity.IngestAsync(data.ToEntity());
        return Ok();
    }
}
