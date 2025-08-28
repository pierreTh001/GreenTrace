using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Documents;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documents;
    public DocumentsController(IDocumentService documents) => _documents = documents;

    [HttpPost]
    [SwaggerOperation(Summary = "Téléverse un document",
        Description = "Ajoute un document et retourne ses métadonnées.")]
    public async Task<IActionResult> Upload(UploadDocumentViewModel document)
    {
        var doc = await _documents.UploadAsync(document.ToEntity());
        return Ok(doc.ToViewModel());
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Récupère un document",
        Description = "Retourne les métadonnées d’un document par identifiant.")]
    public async Task<IActionResult> Download(Guid id)
    {
        var doc = await _documents.GetAsync(id);
        if (doc == null) return NotFound();
        return Ok(doc.ToViewModel());
    }
}
