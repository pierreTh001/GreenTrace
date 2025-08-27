using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _users.GetAllAsync();
        var result = users.Select(u => u.ToViewModel());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user.ToViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateUserViewModel req)
    {
        var result = await _users.RegisterAsync(req.Email, req.Password, req.FirstName, req.LastName);
        return Ok(result.user.ToViewModel());
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateUserViewModel req)
    {
        var user = await _users.UpdateAsync(id, req.FirstName, req.LastName);
        return Ok(user.ToViewModel());
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _users.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _users.ActivateAsync(id);
        return Ok();
    }

    [HttpPut("{id}/preferences")]
    [Authorize]
    public async Task<IActionResult> UpdatePreferences(Guid id, object preferences)
    {
        await _users.UpdatePreferencesAsync(id, preferences);
        return Ok();
    }
}
