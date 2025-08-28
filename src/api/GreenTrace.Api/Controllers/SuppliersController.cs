using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Suppliers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Subscribed")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _suppliers;
    public SuppliersController(ISupplierService suppliers) => _suppliers = suppliers;

    [HttpGet]
    [SwaggerOperation(Summary = "Liste des fournisseurs",
        Description = "Retourne la liste des fournisseurs de la société.")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _suppliers.GetAllAsync();
        return Ok(result.Select(s => s.ToViewModel()));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Crée un fournisseur",
        Description = "Ajoute un nouveau fournisseur et retourne sa représentation.")]
    public async Task<IActionResult> Create(CreateSupplierViewModel supplier)
    {
        var created = await _suppliers.CreateAsync(supplier.ToEntity());
        return Ok(created.ToViewModel());
    }
}
