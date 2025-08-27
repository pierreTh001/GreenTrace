using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.Services;
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
        return Ok(sites);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid companyId, CompanySite site)
    {
        var created = await _sites.CreateAsync(companyId, site);
        return Ok(created);
    }
}
