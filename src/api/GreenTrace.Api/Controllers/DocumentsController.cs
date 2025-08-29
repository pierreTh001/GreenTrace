using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Documents;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using GreenTrace.Api.Infrastructure;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documents;
    private readonly AppDbContext _db;
    public DocumentsController(IDocumentService documents, AppDbContext db)
    {
        _documents = documents;
        _db = db;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Téléverse un document",
        Description = "Ajoute un document et retourne ses métadonnées.")]
    public async Task<IActionResult> Upload(UploadDocumentViewModel document)
    {
        // If associated to a company, ensure membership
        if (document.CompanyId.HasValue)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            var userId = Guid.Parse(userIdStr!);
            var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == document.CompanyId.Value);
            if (!allowed) return Forbid();
        }
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
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        Guid? companyId = doc.CompanyId;
        if (companyId == null && doc.ReportId != null)
        {
            var rep = await _db.Reports.FindAsync(doc.ReportId.Value);
            companyId = rep?.CompanyId;
        }
        if (companyId != null)
        {
            var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId.Value);
            if (!allowed) return Forbid();
        }
        return Ok(doc.ToViewModel());
    }
}
