using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Sites;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
[Authorize]
public class SitesController : ControllerBase
{
    private readonly ISiteService _sites;
    public SitesController(ISiteService sites) => _sites = sites;

    [HttpGet]
    public async Task<IActionResult> Get(Guid companyId)
    {
        var sites = await _sites.GetByCompanyAsync(companyId);
        return Ok(sites.Select(s => s.ToViewModel()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid companyId, CreateSiteViewModel site)
    {
        var created = await _sites.CreateAsync(companyId, site.ToEntity(companyId));
        return Ok(created.ToViewModel());
    }
}
