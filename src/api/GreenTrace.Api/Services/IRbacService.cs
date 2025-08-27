using GreenTrace.Api.Infrastructure.Entities;

namespace GreenTrace.Api.Services;

public interface IRbacService
{
    Task<IEnumerable<Role>> GetRolesAsync();
    Task<IEnumerable<Permission>> GetPermissionsAsync();
    Task AssignRoleAsync(Guid userId, Guid companyId, Guid roleId);
}
