using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Companies;
using GreenTrace.Api.Infrastructure.Entities;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companies;
    public CompaniesController(ICompanyService companies) => _companies = companies;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _companies.GetAllAsync();
        return Ok(result.Select(c => c.ToViewModel()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var company = await _companies.GetByIdAsync(id);
        if (company == null) return NotFound();
        return Ok(company.ToViewModel());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateCompanyViewModel company)
    {
        var created = await _companies.CreateAsync(company.ToEntity());
        return Ok(created.ToViewModel());
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, UpdateCompanyViewModel company)
    {
        var entity = new Company();
        company.MapTo(entity);
        var updated = await _companies.UpdateAsync(id, entity);
        return Ok(updated.ToViewModel());
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _companies.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/parent/{parentId}")]
    [Authorize]
    public async Task<IActionResult> SetParent(Guid id, Guid parentId)
    {
        await _companies.SetParentAsync(id, parentId);
        return Ok();
    }
}
