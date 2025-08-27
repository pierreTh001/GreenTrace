using GreenTrace.Api.Infrastructure;
using GreenTrace.Api.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GreenTrace.Api.Services;

public class RbacService : IRbacService
{
    private readonly AppDbContext _db;

    public RbacService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
        => await _db.Roles.ToListAsync();

    public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        => await _db.Permissions.ToListAsync();

    public async Task AssignRoleAsync(Guid userId, Guid companyId, Guid roleId)
    {
        var existing = await _db.UserCompanyRoles
            .FirstOrDefaultAsync(u => u.UserId == userId && u.CompanyId == companyId && u.RoleId == roleId);
        if (existing == null)
        {
            _db.UserCompanyRoles.Add(new UserCompanyRole
            {
                UserId = userId,
                CompanyId = companyId,
                RoleId = roleId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty
            });
            await _db.SaveChangesAsync();
        }
    }
}
