using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Reports;
using System.Linq;
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
        return Ok(result.Select(r => r.ToViewModel()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var report = await _reports.GetByIdAsync(id);
        if (report == null) return NotFound();
        return Ok(report.ToViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReportViewModel report)
    {
        var created = await _reports.CreateAsync(report.ToEntity());
        return Ok(created.ToViewModel());
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
