using GreenTrace.Api.Infrastructure.Entities;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reports.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var report = await _reports.GetByIdAsync(id);
        if (report == null) return NotFound();
        return Ok(report);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Report report)
    {
        var created = await _reports.CreateAsync(report);
        return Ok(created);
    }

    [HttpPost("{id}/lock")]
    public async Task<IActionResult> Lock(Guid id)
    {
        await _reports.LockAsync(id);
        return Ok();
    }

    [HttpPost("{id}/unlock")]
    public async Task<IActionResult> Unlock(Guid id)
    {
        await _reports.UnlockAsync(id);
        return Ok();
    }

    [HttpPost("{id}/sign-off")]
    public async Task<IActionResult> SignOff(Guid id)
    {
        await _reports.SignOffAsync(id);
        return Ok();
    }
}
