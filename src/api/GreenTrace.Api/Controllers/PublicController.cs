using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/public")]
[AllowAnonymous]
public class PublicController(ICompanyLookupService lookup) : ControllerBase
{
    private readonly ICompanyLookupService _lookup = lookup;

    [HttpGet("companies/lookup")]
    [SwaggerOperation(Summary = "Recherche publique par raison sociale",
        Description = "Suggestions depuis la base Sirene (raison sociale → SIREN/SIRET/NAF/adresse).")]
    public async Task<IActionResult> CompaniesLookup([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Ok(Array.Empty<object>());
        var result = await _lookup.SearchAsync(query);
        return Ok(result);
    }
}

