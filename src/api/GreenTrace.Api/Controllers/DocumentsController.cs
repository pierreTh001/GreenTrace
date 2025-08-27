using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Documents;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documents;
    public DocumentsController(IDocumentService documents) => _documents = documents;

    [HttpPost]
    public async Task<IActionResult> Upload(UploadDocumentViewModel document)
    {
        var doc = await _documents.UploadAsync(document.ToEntity());
        return Ok(doc.ToViewModel());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Download(Guid id)
    {
        var doc = await _documents.GetAsync(id);
        if (doc == null) return NotFound();
        return Ok(doc.ToViewModel());
    }
}
