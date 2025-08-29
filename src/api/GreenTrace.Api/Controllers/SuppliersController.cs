using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Suppliers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GreenTrace.Api.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
[Authorize(Policy = "Subscribed")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _suppliers;
    private readonly AppDbContext _db;
    public SuppliersController(ISupplierService suppliers, AppDbContext db)
    {
        _suppliers = suppliers;
        _db = db;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Liste des fournisseurs",
        Description = "Retourne la liste des fournisseurs de la société.")]
    public async Task<IActionResult> GetAll(Guid companyId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var result = await _suppliers.GetByCompanyAsync(companyId);
        return Ok(result.Select(s => s.ToViewModel()));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Crée un fournisseur",
        Description = "Ajoute un nouveau fournisseur et retourne sa représentation.")]
    public async Task<IActionResult> Create(Guid companyId, CreateSupplierViewModel supplier)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userId = Guid.Parse(userIdStr!);
        var allowed = _db.UserCompanyRoles.Any(u => u.UserId == userId && u.CompanyId == companyId);
        if (!allowed) return Forbid();
        var created = await _suppliers.CreateAsync(companyId, supplier.ToEntity());
        return Ok(created.ToViewModel());
    }
}
