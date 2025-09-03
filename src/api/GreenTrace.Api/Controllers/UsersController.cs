using GreenTrace.Api.Services;
using GreenTrace.Api.Mappers;
using GreenTrace.Api.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;
using GreenTrace.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using GreenTrace.Api.ViewModels.Companies;
using GreenTrace.Api.ViewModels.Reports;
using GreenTrace.Api.Services;

namespace GreenTrace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    private readonly AppDbContext _db;
    private readonly ISubscriptionService _subs;

    public UsersController(IUserService users, AppDbContext db, ISubscriptionService subs)
    {
        _users = users;
        _db = db;
        _subs = subs;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Liste des utilisateurs",
        Description = "Retourne tous les utilisateurs (réservé aux administrateurs).")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _users.GetAllAsync();
        var result = new List<UserSummaryViewModel>();
        var roleMap = await (from usr in _db.UserSystemRoles
                              join r in _db.Roles on usr.RoleId equals r.Id
                              group r.Code by usr.UserId into g
                              select new { UserId = g.Key, Roles = g.ToList() }).ToListAsync();
        foreach (var u in users)
        {
            var roles = roleMap.FirstOrDefault(x => x.UserId == u.Id)?.Roles ?? new List<string>();
            result.Add(new UserSummaryViewModel(u.Id, u.Email, u.FirstName, u.LastName, roles));
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Détails d’un utilisateur",
        Description = "Retourne l’utilisateur correspondant à l’identifiant (admin).")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null) return NotFound();

        var sysRoles = await (from usr in _db.UserSystemRoles
                              join r in _db.Roles on usr.RoleId equals r.Id
                              where usr.UserId == id
                              select r.Code).ToListAsync();

        // Companies where user has a role
        var companies = await (from ucr in _db.UserCompanyRoles
                               where ucr.UserId == id
                               group ucr by ucr.CompanyId into g
                               select g.Key).ToListAsync();

        var companyInfos = new List<UserCompanyInfoViewModel>();
        foreach (var cid in companies)
        {
            var comp = await _db.Companies.FindAsync(cid);
            if (comp == null) continue;
            var roles = await (from ucr in _db.UserCompanyRoles
                               join r in _db.Roles on ucr.RoleId equals r.Id
                               where ucr.UserId == id && ucr.CompanyId == cid
                               select r.Code).ToListAsync();
            var reports = await _db.Reports.Where(r => r.CompanyId == cid).ToListAsync();
            companyInfos.Add(new UserCompanyInfoViewModel(
                comp.ToViewModel(),
                roles,
                reports.Select(r => r.ToViewModel())));
        }

        var detail = new UserDetailViewModel(user.Id, user.Email, user.FirstName, user.LastName, sysRoles, companyInfos);
        return Ok(detail);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Crée un utilisateur",
        Description = "Crée un utilisateur et renvoie sa représentation (admin).")]
    public async Task<IActionResult> Create(CreateUserViewModel req)
    {
        var result = await _users.RegisterAsync(req.Email, req.Password, req.FirstName, req.LastName);
        var roles = result.roles.ToList();
        var vm = new UserSummaryViewModel(result.user.Id, result.user.Email, result.user.FirstName, result.user.LastName, roles);
        return Ok(vm);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Met à jour un utilisateur",
        Description = "Met à jour les informations de l’utilisateur (admin).")]
    public async Task<IActionResult> Update(Guid id, UpdateUserViewModel req)
    {
        var user = await _users.UpdateAsync(id, req.FirstName, req.LastName);
        var sysRoles = await (from usr in _db.UserSystemRoles
                              join r in _db.Roles on usr.RoleId equals r.Id
                              where usr.UserId == id
                              select r.Code).ToListAsync();
        var vm = new UserSummaryViewModel(user.Id, user.Email, user.FirstName, user.LastName, sysRoles);
        return Ok(vm);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Supprime un utilisateur",
        Description = "Supprime l’utilisateur (admin).")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _users.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Supprime mon compte",
        Description = "Supprime immédiatement s'il n'y a pas d'abonnement en cours. Sinon, annule le renouvellement et planifie la suppression à l'échéance d'engagement.")]
    public async Task<IActionResult> DeleteMe()
    {
        var uidStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (uidStr == null) return Unauthorized();
        if (!Guid.TryParse(uidStr.Value, out var userId)) return Unauthorized();

        var current = await _subs.GetCurrentAsync(userId);
        if (current == null)
        {
            await _users.DeleteAsync(userId);
            return NoContent();
        }

        // Cancel if needed and set deletion at engagement end
        if (current.Status == "Active" || current.EndsAt == null)
        {
            current = await _subs.CancelAsync(userId);
        }
        if (current?.EndsAt == null) return Ok(new { scheduledDeletionAt = (DateTimeOffset?)null });
        await _users.MarkForDeletionAsync(userId, current.EndsAt.Value);
        return Ok(new { scheduledDeletionAt = current.EndsAt });
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Active un utilisateur",
        Description = "Active le compte utilisateur (admin).")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _users.ActivateAsync(id);
        return Ok();
    }

    [HttpPut("{id}/preferences")]
    [Authorize]
    [SwaggerOperation(Summary = "Met à jour les préférences",
        Description = "Met à jour les préférences de l’utilisateur connecté.")]
    public async Task<IActionResult> UpdatePreferences(Guid id, object preferences)
    {
        await _users.UpdatePreferencesAsync(id, preferences);
        return Ok();
    }
    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Mon profil",
        Description = "Retourne les informations du profil de l’utilisateur connecté.")]
    public async Task<IActionResult> Me()
    {
        var uidStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (uidStr == null) return Unauthorized();
        if (!Guid.TryParse(uidStr.Value, out var userId)) return Unauthorized();
        var u = await _users.GetByIdAsync(userId);
        if (u == null) return NotFound();
        return Ok(new { id = u.Id, email = u.Email, firstName = u.FirstName, lastName = u.LastName, deletedAt = u.DeletedAt });
    }

    [HttpPut("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Met à jour mon profil",
        Description = "Met à jour prénom et nom pour l’utilisateur connecté.")]
    public async Task<IActionResult> UpdateMe(UpdateUserViewModel req)
    {
        var uidStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (uidStr == null) return Unauthorized();
        if (!Guid.TryParse(uidStr.Value, out var userId)) return Unauthorized();
        var u = await _users.UpdateAsync(userId, req.FirstName, req.LastName);
        return Ok(new { id = u.Id, email = u.Email, firstName = u.FirstName, lastName = u.LastName, deletedAt = u.DeletedAt });
    }

    [HttpPost("me/reactivate")]
    [Authorize]
    [SwaggerOperation(Summary = "Réactive le compte",
        Description = "Supprime le marquage de suppression différée sur le compte utilisateur.")]
    public async Task<IActionResult> ReactivateMe()
    {
        var uidStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (uidStr == null) return Unauthorized();
        if (!Guid.TryParse(uidStr.Value, out var userId)) return Unauthorized();
        await _users.ReactivateAsync(userId);
        return Ok();
    }
}
