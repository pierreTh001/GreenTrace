using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RbacController : ControllerBase
{
    private readonly IRbacService _rbac;
    public RbacController(IRbacService rbac) => _rbac = rbac;

    [HttpGet("roles")]
    [SwaggerOperation(Summary = "Liste des rôles",
        Description = "Retourne l’ensemble des rôles système disponibles.")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _rbac.GetRolesAsync();
        return Ok(roles);
    }

    [HttpGet("permissions")]
    [SwaggerOperation(Summary = "Liste des permissions",
        Description = "Retourne l’ensemble des permissions système.")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _rbac.GetPermissionsAsync();
        return Ok(permissions);
    }

    [HttpPost("assign")]
    [SwaggerOperation(Summary = "Assigne un rôle à un utilisateur",
        Description = "Assigne un rôle à un utilisateur pour une entreprise donnée.")]
    public async Task<IActionResult> Assign(RoleAssignmentRequest req)
    {
        await _rbac.AssignRoleAsync(req.UserId, req.CompanyId, req.RoleId);
        return Ok();
    }

    public record RoleAssignmentRequest(Guid UserId, Guid CompanyId, Guid RoleId);
}
